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
using Noisrev.League.IO.RST.Helpers;
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
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
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
        public string Config { get; set; }

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

                    /* Config is not null ? */
                    if (!string.IsNullOrEmpty(Config) && Config.Length != 0)
                    {
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
        /// Initializes a new instance of the <see cref="RSTFile"/> class and set the version and Type.
        /// </summary>
        /// <param name="version">RST Version</param>
        /// <exception cref="ArgumentException">Invalid Major version.</exception>
        public RSTFile(RVersion version)
        {
            var type = version.GetRType();

            /* Check the type  */
            if (type == null) throw new ArgumentException($"Invalid Major version {(byte)version}. Must be one of 2, 3, 4, 5");

            this.Type = type.Value;
            this.Version = version;
            this.Entries = new Dictionary<ulong, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSTFile"/> class and read content from the stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="leaveOpen">true to leave the stream open after the <see cref="BinaryReader"/> object is disposed; otherwise, false.</param>
        /// <exception cref="ArgumentException">The inputStream does not supports reading.</exception>
        /// <exception cref="ArgumentNullException">outputStream is null.</exception>
        /// <exception cref="InvalidDataException">Invalid RST file.</exception>
        public RSTFile(Stream inputStream, bool leaveOpen)
        {
            if (inputStream == null) throw new ArgumentNullException(nameof(inputStream));
            if (inputStream.CanRead == false) throw new ArgumentException("The inputStream does not supports reading!");

            // Init BinaryReader, use UTF-8
            using (BinaryReader br = new BinaryReader(inputStream, Encoding.UTF8, leaveOpen))
            {
                // Read magic code
                var magic = br.ReadString(3);
                if (magic != Magic)
                {
                    // Invalid magic code
                    throw new InvalidDataException($"Invalid RST file header: {magic}");
                }

                //Set Version
                Version = (RVersion)br.ReadByte();

                // Version 2 and Version 3
                if (Version == RVersion.Ver2 || Version == RVersion.Ver3)
                {
                    // The keys for versions 2 and 3
                    Type = RType.Complex;
                    // Version 2
                    if (Version == RVersion.Ver2)
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

                // Read Hashes
                var hashoffsets = UnsafeReadHashes(br, br.ReadInt32(), Type);

                /* Version less than 5 */
                if (Version < RVersion.Ver5)
                {
                    // Read Mode
                    Mode = (RMode)br.ReadByte();
                }

                // Read Content
                Entries = UnsafeReadContent(br, hashoffsets);
            }
        }

        internal static unsafe Dictionary<ulong, long> UnsafeReadHashes(BinaryReader br, int count, RType type)
        {
            var hashKey = type.ComputeKey();
            var hashesOffsets = new Dictionary<ulong, long>(count);

            var hashesData = br.ReadBytes(count * 8);
            fixed (byte* hashesDataPtr = hashesData)
            {
                var currentPtr = (ulong*)hashesDataPtr;
                var lastPtr = currentPtr + count;

                while (currentPtr < lastPtr)
                {
                    //Read the hash data
                    var item = *currentPtr;

                    // Generate offset
                    var offset = Convert.ToInt64(item >> (int)type);
                    // Generate hash
                    var hash = item & hashKey;

                    // Add entry
                    hashesOffsets[hash] = offset;
                    currentPtr++;
                }
            }

            return hashesOffsets;
        }

        internal static unsafe Dictionary<ulong, string> UnsafeReadContent(BinaryReader br, Dictionary<ulong, long> hashesOffsets)
        {
            var hashesContent = new Dictionary<ulong, string>();

            var remainingData = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
            fixed (byte* contentDataPtr = remainingData)
            {
                foreach (var item in hashesOffsets)
                {
                    var firstPtr = contentDataPtr + item.Value;
                    var currentPtr = firstPtr;

                    while (*++currentPtr != 0x00) ;

                    var length = (int)(currentPtr - firstPtr);
                    hashesContent[item.Key] = length == 0 ? string.Empty : Encoding.UTF8.GetString(firstPtr, length);
                }
            }

            return hashesContent;
        }

        /// <summary>
        /// Write the <see cref="RSTFile"/>.
        /// </summary>
        /// <param name="outputPath">Output path of the RST file.</param> 
        /// <exception cref="ArgumentNullException">outputPath is null.</exception>
        public void Write(string outputPath)
        {
            if (string.IsNullOrEmpty(outputPath))
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            using (var ms = new MemoryStream())
            {
                // Write to MemoryStream
                Write(ms, false);
                // Write All Bytes
                File.WriteAllBytes(outputPath, ms.ToArray());
            }
        }

        /// <summary>
        /// Using an output stream, write the <see cref="RSTFile"/> to that stream.
        /// </summary>
        /// <param name="outputStream">The stream used to output RST file.</param>
        /// <param name="leaveOpen">true to leave the stream open after the <see cref="System.IO.BinaryWriter"/> object is disposed; otherwise, false.</param>
        /// <exception cref="ArgumentException">The outputStream does not support writes.</exception>
        /// <exception cref="ArgumentNullException">outputStream is null.</exception>
        public unsafe void Write(Stream outputStream, bool leaveOpen)
        {
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
            if (outputStream.CanWrite == false) throw new ArgumentException("The outputStream does not supports writing!");

            // Init Binary Writer
            using (BinaryWriter bw = new BinaryWriter(outputStream, Encoding.UTF8, leaveOpen))
            {
                // Used to store header blocks.
                List<byte> headerBlock = new List<byte>();

                // Add the magic code to the header block.
                headerBlock.AddRange(Encoding.ASCII.GetBytes(Magic));

                // Add the version to the header block.
                headerBlock.Add((byte)Version);

                // Version 2
                if (Version == RVersion.Ver2)
                {
                    var hasConfig = !string.IsNullOrEmpty(Config) && Config.Length != 0;
                    /* Config whether there is any content? */
                    headerBlock.Add(Convert.ToByte(hasConfig));

                    // True
                    if (hasConfig)
                    {
                        // Size of Config
                        headerBlock.AddRange(BitConverter.GetBytes(Config.Length));
                        // Content
                        headerBlock.AddRange(Encoding.UTF8.GetBytes(Config));
                    }
                }

                // Add the count to the header block.
                headerBlock.AddRange(BitConverter.GetBytes(this.Entries.Count));

                // Set the hash offset.
                var hashOffset = headerBlock.Count;

                // Used to store data blocks
                List<byte> dataBlock = new List<byte>();

                // Use a dictionary to filter duplicate items.
                var textHashesOffsets = new Dictionary<string, (long, List<ulong>)>();

                // Filter duplicates and add data to the list.
                foreach (var entry in this.Entries)
                {
                    var text = entry.Value;

                    // If there is duplicate content (Text) in the dictionary.
                    if (textHashesOffsets.TryGetValue(text, out var tuple))
                    {
                        // Add the current hash to the list without writing anything. Because there is repetition.
                        tuple.Item2.Add(entry.Key);
                    }
                    else
                    {
                        // offset
                        var offset = dataBlock.Count;
                        // Add the count to the data block.
                        dataBlock.AddRange(Encoding.UTF8.GetBytes(text));
                        // Add the end char to the data block.
                        dataBlock.Add(0x00);

                        // Add to dictionary.
                        textHashesOffsets.Add(text, (offset, new List<ulong>() { entry.Key }));
                    }
                }

                // Loop through the List in the tuple and generate hash data.
                foreach (var tuple in textHashesOffsets.Values)
                {
                    // Compute RST Hash
                    var offset = tuple.Item1;
                    foreach (var item in tuple.Item2)
                    {
                        headerBlock.AddRange(BitConverter.GetBytes(RSTHash.ComputeHash(item, offset, Type)));
                    }
                }

                /* Version less than 5 */
                if (Version < RVersion.Ver5)
                {
                    // Mode
                    headerBlock.Add((byte)Mode);
                }

                // Write Block
                bw.Write(headerBlock.ToArray());
                bw.Write(dataBlock.ToArray());

                // Flush to prevent unwritten data.
                bw.Flush();
            }
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

            if (Version == RVersion.Ver2 && !Config.Equals(other.Config))
            {
                return false;
            }

            if (Type != other.Type || Mode != other.Mode || this.Entries.Count != other.Entries.Count)
            {
                return false;
            }

            foreach (var pair in this.Entries)
            {
                if (other.Entries.TryGetValue(pair.Key, out string value))
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
    }
}