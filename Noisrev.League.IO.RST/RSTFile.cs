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
 *   |     Size:    |       3      |       1      |       4      |      8xn      |       1      |      ...     |
 *   |--------------|--------------|--------------|--------------|---------------|--------------|--------------|
 *   |    Format:   |    String    |     Byte     |     Int32    |     UInt64    |     Byte     |    Entries   |
 *   |--------------|--------------|--------------|--------------|---------------|--------------|--------------|
 *   | Description: |  Magic Code  |     Major    |     Count    | RST hash list |     Minor    |  Entry List  |
 *   |______________|______________|______________|______________|_______________|______________|______________|
 *   
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
 *                                                                                                       ---DateTime : 7.2.2021 --13.14
 */
using Noisrev.League.IO.RST.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static readonly string Magic = "RST";
        /// <summary>
        /// The type of RST used to generate the hash
        /// </summary>
        public RType Type { get; private set; }
        /// <summary>
        /// RST File Font Config, using by RST v2
        /// </summary>
        public string Config { get; private set; }
        /// <summary>
        /// The data segment is located at Position of the current stream
        /// </summary>
        public long DataOffset { get; private set; }
        /// <summary>
        /// Use LazyLoad for items
        /// </summary>
        public bool UseLazyLoad { get; private set; }
        /// <summary>
        /// RST File Version
        /// </summary>
        public global::System.Version Version { get; private set; }
        /// <summary>
        /// Collection of RST entries
        /// </summary>
        public ReadOnlyCollection<RSTEntry> Entries { get; private set; }

        private readonly List<RSTEntry> entries;

        internal Stream DataStream;
        /// <summary>
        /// Initialize the RSTFile class
        /// </summary>
        public RSTFile()
        {
            this.entries = new List<RSTEntry>();
            this.Entries = entries.AsReadOnly();
        }

        /// <summary>
        /// Initialize and set the version and Type
        /// </summary>
        /// <param name="version">RST Version</param>
        public RSTFile(Version version) : this()
        {
            // Version 2 and 3
            if (version.Major == 2 || version.Major == 3)
            {
                // Set the type Complex.
                Type = RType.Complex;
            }
            // Version4
            else if (version.Major == 4)
            {
                // Set the type Simple.
                Type = RType.Simple;
            }
            // Invalid version.
            else
            {
                // An exception is thrown.
                throw new ArgumentException($"Invalid Major version {version.Major}. Must be one of 2,3,4");
            }
            // Set the version.
            this.Version = version;
        }

        /// <summary>
        /// Read the RST file from the stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="leaveOpen">true to leave the stream open after the System.IO.BinaryReader object is disposed; otherwise, false.</param>
        /// <param name="useLazyLoad">Sets whether to use LazyLoad.</param>
        public RSTFile(Stream input, bool leaveOpen, bool useLazyLoad) : this()
        {
            // Set LazyLoad
            UseLazyLoad = useLazyLoad;

            // Init BinaryReader, use UTF-8
            using BinaryReader br = new BinaryReader(input, Encoding.UTF8, leaveOpen);

            // Read magic code
            string magic = br.ReadString(3);
            if (magic != Magic)
            {
                // Invalid magic code
                throw new InvalidDataException($"Invalid RST file header: {magic}");
            }

            //Set Version
            byte major = br.ReadByte();

            // If this is version 2 and version 3
            if (major == 2 || major == 3)
            {
                // The keys for versions 2 and 3
                Type = RType.Complex;
                // Version 2
                if (major == 2)
                {
                    // 0 or 1
                    bool hasConfig = br.ReadBoolean();
                    if (hasConfig) // true
                    {
                        // Config length
                        int length = br.ReadInt32();
                        // Reads the string into the buffer.
                        byte[] buffer = br.ReadBytes(length);

                        // Convert the bytes of the buffer to a UTF-8 string and set it to Config.
                        Config = Encoding.UTF8.GetString(buffer);
                    }
                }
                // Version 3
                // pass
            }
            // If this is version 4
            else if (major == 4)
            {
                // Key for version 4
                Type = RType.Simple;
            }
            // Not equivalent to versions 2, 3, and 4.
            else
            {
                // Invalid or unsupported version and throws an exception.
                throw new InvalidDataException($"Unsupported RST version: {Version}");
            }

            // Set hash key
            ulong hashKey = Type.ComputeKey();
            // Read Count
            int count = br.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                //Read the hash data
                ulong Hashgroup = br.ReadUInt64();

                // Generate offset
                long offset = Convert.ToInt64(Hashgroup >> ((int)Type));
                // Generate hash
                ulong hash = Hashgroup & hashKey;

                // Add entry
                entries.Add(new RSTEntry(this, offset, hash));
            }

            // Read minor
            byte minor = br.ReadByte();

            // Set Version
            Version = new Version(major, minor);

            // Set Data Offset
            DataOffset = br.BaseStream.Position;

            // Set Data Stream
            input.AutoCopy(out DataStream);

            // If doesn't use LazyLoad
            if (!useLazyLoad)
            {
                // Iterate through all the entries
                for (int i = 0; i < count; i++)
                {
                    // Set the content
                    ReadText(entries[i]);
                }
            }
        }
        /// <summary>
        /// Reading content begins at the offset specified in the stream.
        /// </summary>
        /// <param name="entry">Entry to be read</param>
        internal void ReadText(RSTEntry entry)
        {
            // Set the text
            entry.Text = DataStream.ReadStringWithEndByte(entry.Offset, 0x00);
        }
        public void Write(Stream output, bool leaveOpen)
        {
            // Init Binary Writer
            using BinaryWriter bw = new BinaryWriter(output, Encoding.UTF8, leaveOpen);

            // Write Magic Code
            bw.Write(Magic.ToCharArray());

            // Write Major
            bw.Write((byte)Version.Major);

            // Version 2.1
            if (Version.Major == 2 && Version.Minor == 1)
            {
                // Config whether there is any content?
                bool hasConfig = Config.Length == 0;
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
            bw.Write(entries.Count);

            // Set the hash offset.
            long hashOffset = bw.BaseStream.Position;
            // Set the data offset.
            long dataOffset = hashOffset + (entries.Count * 8) + 1; /* hashOffset + hashesSize + (byte)Minor */

            // to dataOffset
            bw.BaseStream.Seek(dataOffset, SeekOrigin.Begin);

            // Initialize dictionary
            // Use a dictionary to filter duplicate items
            Dictionary<string, long> offsets = new Dictionary<string, long>();

            // Write Data
            foreach (var item in entries)
            {
                // Get the Text first. Otherwise, if this is in LazyLoad
                // Like this:
                //------You must set the offset, the starting point, before writing.
                //
                //      item.Offset = bw.BaseStream.Position - dataOffset;
                //
                //------So in Text.get. Since the offset has been reset, on the new offset.
                //------It is likely that the content will not be read and an exception will be thrown.
                //
                //      string text = item.Text;
                string text = item.Text;

                // If there is duplicate content in the dictionary.
                if (offsets.ContainsKey(text))
                {
                    // Set the offset. And do not write the content. Because there's repetition.
                    item.Offset = offsets[text];
                }
                // No repeat
                else
                {
                    // Write Offset
                    item.Offset = bw.BaseStream.Position - dataOffset;
                    // Write Text
                    bw.Write(Encoding.UTF8.GetBytes(text));
                    // Write End Byte
                    bw.Write((byte)0x00);

                    // Add to dictionary
                    offsets.Add(text, item.Offset);
                }
            }
            // To hashOffset
            bw.BaseStream.Seek(hashOffset, SeekOrigin.Begin);
            // Write hashes
            foreach (var item in entries)
            {
                // Write RST Hash
                bw.Write(RSTHash.ComputeHash(item.Hash, item.Offset, Type));
            }
            // Write Minor
            bw.Write((byte)Version.Minor);
            // Flush to prevent unwritten data
            bw.Flush();

            this.Dispose();
            // Set Data Stream
            output.AutoCopy(out DataStream);
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
                (DataStream as IDisposable)?.Dispose();
            }

            DataStream = null;
        }
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (DataStream != null)
            {
                await DataStream.DisposeAsync().ConfigureAwait(false);
            }
            DataStream = null;
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
            if (Version.Major == 2 && Version.Minor == 1 && !Config.Equals(other.Config))
            {
                return false;
            }
            if (Type != other.Type || entries.Count != other.entries.Count)
            {
                return false;
            }
            for (int i = 0; i < entries.Count; i++)
            {
                if (!entries[i].Equals(other.entries[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
