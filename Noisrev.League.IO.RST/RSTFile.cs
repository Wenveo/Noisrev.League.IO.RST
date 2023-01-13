/*
 * Copyright (c) 2021 - 2023 Noisrev
 * All rights reserved.
 *
 * This source code is distributed under an MIT license. 
 * LICENSE file in the root directory of this source tree.
 */

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

using Noisrev.League.IO.RST.Helpers;
using Noisrev.League.IO.RST.Unsafe;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Noisrev.League.IO.RST
{
    /// <summary>
    /// Riot String Table File.
    /// </summary>
    public class RSTFile : IEquatable<RSTFile>
    {
        /// <summary>
        /// Load a RST file from a file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>An instance of the <see cref="RSTFile"/> that was successfully read.</returns>
        /// <exception cref="ArgumentNullException">filePath is null.</exception>
        /// <exception cref="FileNotFoundException">File not found.</exception>
        public static RSTFile Load(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            return new RSTFile(File.OpenRead(filePath), false);
        }

        /// <summary>
        /// Magic Code
        /// </summary>
        public const string Magic = "RST";

        /// <summary>
        /// RST File Version.
        /// </summary>
        public RVersion Version { get; }

        /// <summary>
        /// RST File Font Config, using by RST v2.
        /// </summary>
        public string? Config { get; set; }

        /// <summary>
        /// The data segment is located at Position of the current stream.
        /// </summary>
        public long DataOffset
        {
            get
            {
                /* Magic Code(3) + Version(1) */
                long offset = 4;

                // Version 2
                if (Version == RVersion.Ver2)
                {
                    /* hasConfig? (1) boolean */
                    offset += 1;

#if NETFRAMEWORK
#pragma warning disable CS8602
#endif
                    /* Config is not null ? */
                    if (!string.IsNullOrEmpty(Config) && Config.Length > 0)
                    {
#if NETFRAMEWORK
#pragma warning restore CS8602
#endif
                        /* size(int) + strlen */
                        offset += 4 + Config.Length;
                    }
                }

                /* count (4 bytes) +  8 * Count  ***/
                offset += 4 + (8 * this.Entries.Count);

                /* Version less than 5 */
                if (Version < RVersion.Ver5)
                {
                    offset += 1;
                }

                /* Return the offset */
                return offset;
            }
        }

        /// <summary>
        /// The type of RST used to generate the hash.
        /// </summary>
        public RType Type { get; }

        /// <summary>
        /// Mode of the RST.
        /// </summary>
        public RMode Mode { get; }

        /// <summary>
        /// Entries of the RST File.
        /// </summary>
        public Dictionary<ulong, string> Entries { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSTFile"/> class.
        /// </summary>
        private RSTFile()
        {
            this.Entries = new Dictionary<ulong, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSTFile"/> class and set the version and Type.
        /// </summary>
        /// <param name="version">RST Version</param>
        /// <exception cref="ArgumentException">Invalid Major version.</exception>
        public RSTFile(RVersion version) : this()
        {
            /* Check the type  */

            this.Type = version.GetRType() ?? throw new ArgumentException($"Invalid Major version {(byte)version}. Must be one of 2, 3, 4, 5"); ;
            this.Version = version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSTFile"/> class and read content from the <paramref name="inputStream"/>.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="leaveOpen">true to leave the stream open after the <see cref="BinaryReader"/> object is disposed; otherwise, false.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidDataException"/>
        public RSTFile(Stream inputStream, bool leaveOpen) : this()
        {
            if (inputStream == null) throw new ArgumentNullException(nameof(inputStream));
            if (!inputStream.CanRead) throw new ArgumentException("The inputStream does not supports reading!");

            //Read all bytes from the stream and create a reader
            var bytesReader = BytesReader.Create(inputStream.ReadToEnd(), Encoding.UTF8);

            // Read magic code
            var magic = bytesReader.ReadString(3);
            if (magic != Magic)
            {
                // Invalid magic code
                throw new InvalidDataException($"Invalid RST file header: {magic}");
            }

            //Set the Version
            Version = (RVersion)bytesReader.ReadByte();

            // Version 2 and Version 3
            if (Version == RVersion.Ver2 || Version == RVersion.Ver3)
            {
                // The keys for versions 2 and 3
                Type = RType.Complex;
                // Version 2
                if (Version == RVersion.Ver2)
                {
                    // 0 or 1
                    var hasConfig = bytesReader.ReadBoolean();
                    if (hasConfig) // true
                    {
                        // Config length
                        var configLength = bytesReader.ReadInt32();
                        // Read the Config.
                        Config = bytesReader.ReadString(configLength);
                    }
                }
                // Version 3
                // pass
            }
            // If this is version 4 or version 5
            else if (Version == RVersion.Ver4 || Version == RVersion.Ver5)
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

            // Set the hash key
            var hashKey = Type.ComputeKey();

            // Read the count
            var count = bytesReader.ReadInt32();

            // Used to save hashes and offsets
            var hashesOffsets = new Dictionary<ulong, long>(count);

            // Read Hashes
            for (var i = 0; i < count; i++)
            {
                //Read the hash data
                var hashData = bytesReader.ReadUInt64();

                // Generate offset
                var offset = Convert.ToInt64(hashData >> (int)Type);

                // Generate hash
                var hash = hashData & hashKey;

                // Add entry
                hashesOffsets[hash] = offset;
            }

            // Version less than 5
            if (Version < RVersion.Ver5)
            {
                // Read Mode
                Mode = (RMode)bytesReader.ReadByte();
            }

            // Current Position
            var position = bytesReader.Position;

            // Used to filter duplicate text
            var offsetTexts = new Dictionary<long, string>(count);

            // Read Content
            foreach (var item in hashesOffsets)
            {
                // Try to get a string from the dictionary.
                // If false, the string is read and added to the dictionary.
                // But if true, the same text is fetched from the dictionary (to avoid reading the string multiple times).
                if (!offsetTexts.TryGetValue(item.Value, out var value))
                {
                    // Reads the string from position + offset
                    value = bytesReader.ReadStringWithOffset((int)(position + item.Value));
                    // Add the offset and text to the Dictionary
                    offsetTexts.Add(item.Value, value);
                }
                // Add the key and value.
                this.Entries[item.Key] = value;
            }

            if (!leaveOpen) inputStream.Close();
        }

        /// <summary>
        /// Write the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="outputPath">Output path of the RST file.</param> 
        /// <exception cref="ArgumentNullException">outputPath is null.</exception>
        public void Write(string outputPath)
        {
            if (string.IsNullOrEmpty(outputPath)) throw new ArgumentNullException(nameof(outputPath));

            using var ms = new MemoryStream();
            // Write to MemoryStream
            Write(ms, false);
            // Write All Bytes
            File.WriteAllBytes(outputPath, ms.ToArray());

            /// <summary>
            /// Using an output stream, write the <see cref="RSTFile"/> to that stream.
            /// </summary>
            /// <param name="outputStream">The stream used to output RST file.</param>
            /// <param name="leaveOpen">true to leave the stream open after the <see cref="System.IO.BinaryWriter"/> object is disposed; otherwise, false.</param>
            /// <exception cref="ArgumentException">The outputStream does not support writes.</exception>
            /// <exception cref="ArgumentNullException">outputStream is null.</exception>
            public void Write(Stream outputStream, bool leaveOpen)
        {
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
            if (outputStream.CanWrite == false) throw new ArgumentException("The outputStream does not supports writing!");

            // Used to write the header block (DataOffset = HeaderSize). 
            var bytesWriter = BytesWriter.Create((int)DataOffset);

            // Write the Magic Code.
            bytesWriter.Write(new byte[3] { 0x52, 0x53, 0x54 });

            // Write the Version.
            bytesWriter.Write((byte)Version);

            // Version 2
            if (Version == RVersion.Ver2)
            {
#if NETFRAMEWORK
#pragma warning disable CS8602
#endif
                // If True, Write the Config.
                if (!string.IsNullOrEmpty(Config) && Config.Length > 0)
                {
#if NETFRAMEWORK
#pragma warning restore CS8602
#endif
                    // Write the boolean value.
                    bytesWriter.Write(true);
                    // Size of Config
                    bytesWriter.Write(Config.Length);
                    // Content
                    bytesWriter.Write(Config, Encoding.UTF8);
                }
                else
                {
                    // Write the boolean value.
                    bytesWriter.Write(false);
                }
            }
            // Current Count
            var count = this.Entries.Count;

            // Write the Count
            bytesWriter.Write(count);

            // Used to write blocks of data.
            var dataWriter = BytesWriter.Create(count * 256);

            // Use a dictionary to filter duplicate items.
            var textOffsets = new Dictionary<string, long>(count);

            // Filter duplicates and add data to the list.
            foreach (var entry in Entries)
            {
                var text = entry.Value;

                // Try to get a string from the dictionary.
                // If false, add the current string and write the current string to the data block.
                if (!textOffsets.TryGetValue(text, out var offset))
                {
                    // Current offset
                    offset = dataWriter.Position;

                    // Write the Text
                    dataWriter.Write(text, Encoding.UTF8, true);

                    // Add the text and offset to the Dictionary
                    textOffsets.Add(text, offset);
                }
                // Write the Hash
                bytesWriter.Write(RSTHash.ComputeHash(entry.Key, offset, Type));
            }

            // Version less than 5.
            if (Version < RVersion.Ver5)
            {
                // Mode
                bytesWriter.Write((byte)Mode);
            }

            // Write Blocks
            outputStream.Write(bytesWriter.Buffer, 0, bytesWriter.Length);
            outputStream.Write(dataWriter.Buffer, 0, dataWriter.Length);

            // End of Write
            if (leaveOpen)
                outputStream.Flush();
            else
                outputStream.Close();
        }

        /// <summary>
        ///  Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(RSTFile? other)
        {
            if (other == null)
            {
                return false;
            }

            if (!Version.Equals(other.Version))
            {
                return false;
            }

            if (Version == RVersion.Ver2 && !StringComparer.Ordinal.Equals(Config, other.Config))
            {
                return false;
            }

            if (Type != other.Type || Mode != other.Mode || this.Entries.Count != other.Entries.Count)
            {
                return false;
            }

            foreach (var pair in this.Entries)
            {
                if (other.Entries.TryGetValue(pair.Key, out string? value))
                {
                    if (!pair.Value.Equals(value))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        => Equals(obj as RSTFile);

        /// <inheritdoc />
        public override int GetHashCode()
        => Version.GetHashCode() ^
               Type.GetHashCode() ^
                    Mode.GetHashCode() ^
                        Entries.GetHashCode();

    }
}