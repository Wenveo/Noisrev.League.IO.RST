/*  This is a RST (Riot String Table) file class.
 *  
 *  The RST file is a League of Legends file used to store a list of strings.
 *  
 *  They are often used to store text content in different language versions so that League of Legends can reference and switch between different languages.
 *  
 *  The RST file is usually located in the "DATA/Menu" directory.
 *  
 *  Like: "DATA/Menu/fontconfig_en_us.txt", "DATA/Menu/bootstrap_zh_cn.stringtable".
 *  
 *  
 *  
 *  The file structure of the RST is as follows:
 *  
 *   
 *   ___________________________________________________________________________________________________________
 *   |     Pos:     |       0      |       3      |       4      |       8       |      ...     |      ...     |
 *   |--------------|--------------|--------------|--------------|---------------|--------------|--------------|
 *   |     Size:    |       3      |       1      |       4      |      8xN      |       1      |      ...     |
 *   |--------------|--------------|--------------|--------------|---------------|--------------|--------------|
 *   |    Format:   |    String    |     Byte     |     Int32    |     UInt64    |     Byte     |    Entries   |
 *   |--------------|--------------|--------------|--------------|---------------|--------------|--------------|
 *   | Description: |  Magic Code  |    Version   |     Count    | RST hash list |     Mode     |  Entry List  |
 *   |______________|______________|______________|______________|_______________|______________|______________|
 *
 *   *** "Mode" was deprecated in version 5 ***
 *   
 *  The entry structure:
 *                               ______________________________________________
 *                               |     Size:    |       ?      |       1      |
 *                               |--------------|--------------|--------------|
 *                               |    Format:   |    String    |     Byte     |
 *                               |--------------|--------------|--------------|
 *                               | Description: |    Content   |   End Byte   | // The end byte is always 0x00
 *                               |______________|______________|______________| // Like char* or char[] in C, always ending with 0x00 ('\0')
 *                               
 *                                                                                                       ---Author   : Noisrev(晚风✨)
 *                                                                                                       ---Email    : Noisrev@outlook.com
 *                                                                                                       ---DateTime : 7.2.2021 --13:14
 */

#pragma warning disable CS1591
using Noisrev.League.IO.RST.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noisrev.League.IO.RST
{
    /// <summary>
    /// Riot String Table File
    /// </summary>
    public class RSTFile : IAsyncDisposable, IDisposable, IEquatable<RSTFile>
    {
        /// <summary>
        /// Magic Code
        /// </summary>
        public const string Magic = "RST";

        /// <summary>
        /// RST File Version
        /// </summary>
        public byte Version { get; }

        /// <summary>
        /// RST File Font Config, using by RST v2
        /// </summary>
        public string Config { get; private set; }

        /// <summary>
        /// The data segment is located at Position of the current stream
        /// </summary>
        public long DataOffset
        {
            get
            {
                /* Magic Code(3) + Version(1) */
                long offset = 4;

                // Version 2
                if (Version == 2)
                {
                    /* hasConfig? (1) boolean */
                    offset += 1;

                    /* Config is not null ? */
                    if (Config is { } && Config.Length != 0)
                    {
                        /* size(int) + strlen */
                        offset += 4 + Config.Length;
                    }
                }

                /* count (4 bytes) +  8 * Count  ***/
                offset += 4 + (8 * _entries.Count);

                /* Version less than 5 */
                if (Version < 5)
                {
                    offset += 1;
                }

                /* Return the offset */
                return offset;
            }
        }

        /// <summary>
        /// The type of RST used to generate the hash
        /// </summary>
        public RType Type { get; }

        /// <summary>
        /// Mode of the RST
        /// </summary>
        public RMode Mode { get; }

        /// <summary>
        /// Collection of RST entries
        /// </summary>
        public ReadOnlyCollection<RSTEntry> Entries { get; }

        /// <summary>
        /// Private list.
        /// </summary>
        private readonly List<RSTEntry> _entries;

        /// <summary>
        /// The stream that stores the RST data segment.
        /// </summary>
        private Stream _dataStream;

        /// <summary>
        /// Initialize the RSTFile class
        /// </summary>
        private RSTFile()
        {
            // Initialize entries
            this._entries = new List<RSTEntry>();
            // Set Entries to read-only
            this.Entries = _entries.AsReadOnly();
        }

        /// <summary>
        /// Initialize and set the version and Type
        /// </summary>
        /// <param name="version">RST Version</param>
        public RSTFile(byte version) : this()
        {
            var type = version.GetRType();

            /* Check the type  */
            if (type == null) throw new ArgumentException($"Invalid Major version {version}. Must be one of 2, 3, 4, 5");

            this.Type = Type;
            this.Version = version;
        }

        /// <summary>
        /// Read the RST file from the stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="leaveOpen">true to leave the stream open after the <see cref="System.IO.BinaryReader"/> object is disposed; otherwise, false.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="DecoderExceptionFallback"></exception>
        /// <exception cref="EndOfStreamException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public RSTFile(Stream input, bool leaveOpen) : this()
        {
            // Init BinaryReader, use UTF-8
            using var br = new BinaryReader(input, Encoding.UTF8, leaveOpen);

            // Read magic code
            var magic = br.ReadString(3);
            if (magic != Magic)
            {
                // Invalid magic code
                throw new InvalidDataException($"Invalid RST file header: {magic}");
            }

            //Set Version
            Version = br.ReadByte();

            // Version 2 and Version 3
            if (Version == 2 || Version == 3)
            {
                // The keys for versions 2 and 3
                Type = RType.Complex;
                // Version 2
                if (Version == 2)
                {
                    // 0 or 1
                    var hasConfig = br.ReadBoolean();
                    if (hasConfig) // true
                    {
                        // Config length
                        var length = br.ReadInt32();
                        // Read the Config.
                        Config = br.ReadString(length);
                    }
                }
                // Version 3
                // pass
            }
            // If this is version 4 or version 5
            else if (Version == 4 || Version == 5)
            {
                // Key for version 4 and 5
                Type = RType.Simple;
            }
            // Not equivalent to versions 2, 3, 4, 5.
            else
            {
                // Invalid or unsupported version and throws an exception.
                throw new InvalidDataException($"Unsupported RST version: {Version}");
            }

            // Set hash key
            var hashKey = Type.ComputeKey();
            // Read Count
            var count = br.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                //Read the hash data
                var hashGroup = br.ReadUInt64();

                // Generate offset
                var offset = Convert.ToInt64(hashGroup >> (int) Type);
                // Generate hash
                var hash = hashGroup & hashKey;

                // Add entry
                _entries.Add(new RSTEntry(offset, hash));
            }

            /* Version less than 5 */ 
            if (Version < 5)
            {
                // Read Mode
                Mode = (RMode) br.ReadByte();
            }

            // Set Data Stream
            input.AutoCopy(out _dataStream);

            // Iterate through all the entries
            for (var i = 0; i < count; i++)
            {
                // Set the content
                ReadText(_entries[i]);
            }
        }

        /// <summary>
        /// Add entry with key and value.
        /// </summary>
        /// <param name="key">The hash key</param>
        /// <param name="value">The content</param>
        public void AddEntry(string key, string value)
        {
            AddEntry(RSTHash.ComputeHash(key, Type), value);
        }

        /// <summary>
        /// Add entry with hash and value.
        /// </summary>
        /// <param name="hash">The hash</param>
        /// <param name="value">The content</param>
        public void AddEntry(ulong hash, string value)
        {
            _entries.Add(new RSTEntry(hash, value));
        }

        /// <summary>
        /// Find the entry that matches the hash.
        /// </summary>
        /// <param name="hash">The hash</param>
        /// <returns>If it does not exist in the list, return null.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public RSTEntry Find(ulong hash)
        {
            return _entries.Find(x => x.Hash == hash);
        }

        /// <summary>
        /// Inserts an element into the <see cref="List{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="entry">The item</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void Insert(int index, RSTEntry entry)
        {
            _entries.Insert(index, entry);
        }

        /// <summary>
        /// Remove all items that match hash.
        /// </summary>
        /// <param name="hash">The hash</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Remove(ulong hash)
        {
            _entries.RemoveAll(x => x.Hash == hash);
        }

        /// <summary>
        /// Remove the entry.
        /// </summary>
        /// <param name="entry">The entry</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the <see cref="List{T}"/></returns>
        public bool Remove([NotNull] RSTEntry entry)
        {
            return _entries.Remove(entry);
        }

        /// <summary>
        /// Removes the entry at the specified index
        /// </summary>
        /// <param name="index">The index</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void RemoveAt(int index)
        {
            _entries.RemoveAt(index);
        }

        /// <summary>
        /// Reading content begins at the offset specified in the stream.
        /// </summary>
        /// <param name="entry">Entry to be read</param>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="DecoderExceptionFallback"></exception>
        private void ReadText(RSTEntry entry)
        {
            // Set the text
            entry.Text = _dataStream.ReadStringWithEndByte(entry.Offset, 0x00);
        }

        /// <summary>
        /// Replace the matching items in the entire entry. And replace them.
        /// </summary>
        /// <param name="oldText">To Replace</param>
        /// <param name="newText">Replace to</param>
        /// <param name="caseSensitive">Case sensitive</param>
        /// <exception cref="ArgumentNullException"/>
        public void ReplaceAll([NotNull] string oldText, [NotNull] string newText, bool caseSensitive = false)
        {
            // Set a list
            var list = caseSensitive
                ? _entries.Where(x => x.Text.Contains(oldText))
                : _entries.Where(x => x.Text.ToLower().Contains(oldText.ToLower()));

            foreach (var item in list)
            {
                // Set Text
                item.Text = newText;
            }
        }

        /// <summary>
        /// Set the configuration.
        /// </summary>
        /// <param name="conf">The config</param>
        /// <returns>It must be version 2.1 to set the configuration. Set to return true on success or false on failure.</returns>
        public bool SetConfig(string conf)
        {
            // Version 2
            if (Version == 2)
            {
                // Set the config
                Config = conf;
                // Return
                return true;
            }
            // Not version 2
            else
            {
                // Return
                return false;
            }
        }

        /// <summary>
        /// Using an output stream, write the RST to that stream.
        /// </summary>
        /// <param name="output">The output stream.</param>
        /// <param name="leaveOpen">true to leave the stream open after the  <see cref="System.IO.BinaryWriter"/> object is disposed; otherwise, false.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="EncoderFallbackException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="OverflowException"/>
        /// <exception cref="IOException"/>
        public void Write([NotNull]Stream output, bool leaveOpen)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));
            // Init Binary Writer
            using var bw = new BinaryWriter(output, Encoding.UTF8, leaveOpen);

            // Write Magic Code
            bw.Write(Magic.ToCharArray());

            // Write Version
            bw.Write(Version);

            // Version 2
            if (Version == 2)
            {
                var hasConfig = Config is { } && Config.Length != 0;
                /* Config whether there is any content? */
                bw.Write(hasConfig);

                // True
                if (hasConfig)
                {
                    // Write Config
                    {
                        // Write Size
                        bw.Write(Config.Length);
                        // Write Content
                        bw.Write(Config.ToArray());
                    }
                }
            }

            // Write Count
            bw.Write(_entries.Count);

            // Set the hash offset.
            var hashOffset = bw.BaseStream.Position;
            // Set the data offset.
            var dataOffset = hashOffset + (_entries.Count * 8) + (Version < 5 ? 1 : 0); /* hashOffset + hashesSize + (byte)Mode */

            // Go to the dataOffset
            bw.BaseStream.Seek(dataOffset, SeekOrigin.Begin);

            // Initialize dictionary
            // Use a dictionary to filter duplicate items
            var offsets = new Dictionary<string, long>();

            // Write Data
            foreach (var entry in _entries)
            {
                var text = entry.Text;

                // If there is duplicate content in the dictionary.
                if (offsets.ContainsKey(text))
                {
                    // Set the offset. And do not write the content. Because there's repetition.
                    entry.Offset = offsets[text];
                }
                // No repeat
                else
                {
                    // Write Offset
                    entry.Offset = bw.BaseStream.Position - dataOffset;
                    // Write Text
                    bw.Write(Encoding.UTF8.GetBytes(text));
                    // Write End Byte
                    bw.Write((byte) 0x00);

                    // Add to dictionary
                    offsets.Add(text, entry.Offset);
                }
            }

            // Go to the hashOffset
            bw.BaseStream.Seek(hashOffset, SeekOrigin.Begin);
            // Write hashes
            foreach (var entry in _entries)
            {
                // Write RST Hash
                bw.Write(RSTHash.ComputeHash(entry.Hash, entry.Offset, Type));
            }

            /* Version less than 5 */ 
            if (Version < 5)
            {
                // Write Mode
                bw.Write((byte) Mode);
            }

            // Flush to prevent unwritten data
            bw.Flush();

            // Dispose
            this.Dispose();
            // Set Data Stream
            output.AutoCopy(out _dataStream);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                (_dataStream as IDisposable)?.Dispose();
            }

            _dataStream = null;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_dataStream != null)
            {
                await _dataStream.DisposeAsync().ConfigureAwait(false);
            }

            _dataStream = null;
        }

        public bool Equals(RSTFile other)
        {
            if (other == null)
            {
                return false;
            }

            if (!Version.Equals(other.Version))
            {
                return false;
            }

            if (Version == 2 && !Config.Equals(other.Config))
            {
                return false;
            }

            if (Type != other.Type || Mode != other.Mode || _entries.Count != other._entries.Count)
            {
                return false;
            }

            return !_entries.Where((t, i) => !t.Equals(other._entries[i])).Any();
        }
    }
}