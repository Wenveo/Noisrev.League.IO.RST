// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

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

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Noisrev.League.IO.RST.Helpers;
using Noisrev.League.IO.RST.Unsafe;

namespace Noisrev.League.IO.RST;

/// <summary>
/// Riot String Table File.
/// </summary>
public class RSTFile : IEquatable<RSTFile>
{
    /// <summary>
    /// Load <see cref="RSTFile"/> from the local file.
    /// </summary>
    /// <param name="filePath">The local file path.</param>
    /// <returns>Instance of the <see cref="RSTFile"/> that was read successfully.</returns>
    /// <exception cref="ArgumentNullException">filePath is null.</exception>
    /// <exception cref="FileNotFoundException">File not found.</exception>
    public static RSTFile Load(string filePath)
    {
        return new RSTFile(File.OpenRead(filePath), false);
    }

    /// <summary>
    /// The magic code of <see cref="RSTFile"/>.
    /// </summary>
    public static readonly byte[] MagicCode = { 0x52, 0x53, 0x54 };

    /// <summary>
    /// File version of the rst file.
    /// </summary>
    public RVersion Version { get; }

    /// <summary>
    /// Font config of the rst file. using by RST v2.
    /// </summary>
    public string? Config { get; set; }

    /// <summary>
    /// The offset of the string data in the rst file.
    /// </summary>
    public int DataOffset
    {
        get
        {
            /* Magic Code(3) + Version(1) */
            var offset = 4;

            // Version 2
            if (Version == RVersion.Ver2)
            {
                /* hasConfig? (1) boolean */
                offset += 1;

#if !NETCOREAPP && !NETSTANDARD2_1_OR_GREATER
#pragma warning disable CS8602
#endif
                if (!string.IsNullOrEmpty(Config) && Config.Length > 0)
                {
#if !NETCOREAPP && !NETSTANDARD2_1_OR_GREATER
#pragma warning restore CS8602
#endif
                    /* size(int) + strlen */
                    offset += 4 + Config.Length;
                }
            }

            /* Count (4 bytes) + (8 bytes * Count) */
            offset += 4 + (8 * Entries.Count);

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
    /// The type of the <see cref="RSTFile"/> used to generate the hash.
    /// </summary>
    public RType Type { get; }

    /// <summary>
    /// The mode of the <see cref="RSTFile"/>.
    /// </summary>
    public RMode Mode { get; }

    /// <summary>
    /// The entries for hashes and strings of the <see cref="RSTFile"/>.
    /// </summary>
    public Dictionary<ulong, string> Entries { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RSTFile"/> class.
    /// </summary>
    private RSTFile()
    {
        Entries = new Dictionary<ulong, string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RSTFile"/> class based on the specified <see cref="RVersion"/>.
    /// </summary>
    /// <param name="version">RST Version</param>
    /// <exception cref="ArgumentException">Invalid Major version.</exception>
    public RSTFile(RVersion version) : this()
    {
        /* Check the type  */

        Type = version.GetRType() ?? throw new ArgumentException($"Invalid Major version {(byte)version}. Must be one of 2, 3, 4, 5"); ;
        Version = version;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RSTFile"/> class based on the specified <see cref="Stream"/>.
    /// </summary>
    /// <param name="inputStream">The input stream.</param>
    /// <param name="leaveOpen">true to leave the stream open after the <see cref="BinaryReader"/> object is disposed; otherwise, false.</param>
    /// <exception cref="ArgumentException">"The <paramref name="inputStream"/> does not supports reading!"</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="inputStream"/> is null.</exception>
    /// <exception cref="InvalidDataException">Invalid RST file header.</exception>
    public RSTFile(Stream inputStream, bool leaveOpen) : this()
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(nameof(inputStream));
#else
        if (inputStream == null)
        {
            throw new ArgumentNullException(nameof(inputStream));
        }
#endif
        if (!inputStream.CanRead)
        {
            throw new ArgumentException("The inputStream does not supports reading!");
        }

        // Create a new bytes reader with encoding.
        using var bytesReader = BytesReader.Create(inputStream, Encoding.UTF8);

        // Read the magic code of the rst file.
        var magicCode = bytesReader.Read(3);
        if (!magicCode.SequenceEqual(MagicCode))
        {
            // Invalid magic code
            throw new InvalidDataException($"Invalid RST file header: '{{ 0x{magicCode[0]:X}, 0x{magicCode[1]:X}, 0x{magicCode[2]:X} }}'");
        }

        // Read the version of the rst file.
        Version = (RVersion)bytesReader.ReadByte();

        // Version 2 and Version 3
        if (Version is RVersion.Ver2 or RVersion.Ver3)
        {
            // The type key for version 2 and 3
            Type = RType.Complex;

            // Version 2
            if (Version is RVersion.Ver2)
            {
                var hasConfig = bytesReader.ReadBoolean();
                if (hasConfig)
                {
                    // Read the length of the font config.
                    var configLength = bytesReader.ReadInt32();
                    // Read the font config by length.
                    Config = bytesReader.ReadString(configLength);
                }
            }
            // Version 3
            // pass
        }
        // If this is version 4 or version 5
        else if (Version is RVersion.Ver4 or RVersion.Ver5)
        {
            // The type key for version 4 and 5
            Type = RType.Simple;
        }
        // Not equivalent to versions 2, 3, 4, 5
        else
        {
            // Invalid or unsupported file version and then throw a exception.
            throw new InvalidDataException($"Unsupported RST version: {Version}");
        }

        // Set the hash key
        var hashKey = Type.ComputeKey();

        // Read the count of hash data
        var count = bytesReader.ReadInt32();

        // Used to store the hashes and offsets
        var hashesOffsets = ArrayPool<ValueTuple<ulong, int>>.Shared.Rent(count);

        try
        {
            // Read hash data
            for (var i = 0; i < count; i++)
            {
                // Read the hash data
                var hashData = bytesReader.ReadUInt64();

                // Generate offset
                var offset = (int)(hashData >> (int)Type);

                // Generate hash
                var hash = hashData & hashKey;

                hashesOffsets[i].Item1 = hash;
                hashesOffsets[i].Item2 = offset;
            }

            // Version less than 5
            if (Version < RVersion.Ver5)
            {
                Mode = (RMode)bytesReader.ReadByte();
            }

            // The position of the first string data.
            var dataOffset = bytesReader.Position;

            // Used to filter duplicate text
            var offsetToText = new Dictionary<int, string>(count);

            // Read Strings
            for (var j = 0; j < count; j++)
            {
                // Get the element of the current index
                var hashAndOffset = hashesOffsets[j];

                // Try to get a string from the dictionary.
                // If false, the string is read and added to the dictionary.
                // But if true, get the same text from the dictionary (to avoid reading the string multiple times).
                if (!offsetToText.TryGetValue(hashAndOffset.Item2, out var value))
                {
                    // Reads the string from position + offset
                    value = bytesReader.ReadStringByOffset(dataOffset + hashAndOffset.Item2);
                    // Add the offset and text to the Dictionary
                    offsetToText.Add(hashAndOffset.Item2, value);
                }

                // Add the key and value.
                Entries[hashAndOffset.Item1] = value;
            }
        }
        finally
        {
            ArrayPool<ValueTuple<ulong, int>>.Shared.Return(hashesOffsets);
        }

        if (!leaveOpen)
        {
            // Close the input stream.
            inputStream.Close();
        }
    }

    /// <summary>
    /// Creates a new file, writes the binary data of the <see cref="RSTFile"/> to the file, and then closes the file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="outputPath">The file to write to.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="outputPath"/> is null.</exception>
    public void Write(string outputPath)
    {
        Write(File.Create(outputPath), false);
    }

    /// <summary>
    /// Using an output stream, write the <see cref="RSTFile"/> to that <see cref="Stream"/>.
    /// </summary>
    /// <param name="outputStream">The output stream.</param>
    /// <param name="leaveOpen">true to leave the stream open after the <see cref="BinaryWriter"/> object is disposed; otherwise, false.</param>
    /// <exception cref="ArgumentException">The <paramref name="outputStream"/> does not support writes.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="outputStream"/> is null.</exception>
    public void Write(Stream outputStream, bool leaveOpen)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(nameof(outputStream));
#else
        if (outputStream == null)
        {
            throw new ArgumentNullException(nameof(outputStream));
        }
#endif
        if (!outputStream.CanWrite)
        {
            throw new ArgumentException("Stream was not writable.", nameof(outputStream));
        }

        // Used to write the file header and hashes
        // (0 to dataOffset) = file header + hashes
        using var bytesWriter = BytesWriter.Create(DataOffset, Encoding.UTF8);

        // Write the Magic Code.
        bytesWriter.Write(MagicCode);

        // Write the Version.
        bytesWriter.Write((byte)Version);

        // Version 2
        if (Version == RVersion.Ver2)
        {
#if !NETCOREAPP && !NETSTANDARD2_1_OR_GREATER
#pragma warning disable CS8602
#endif
            // If true, Write the font config.
            if (!string.IsNullOrEmpty(Config) && Config.Length > 0)
            {
#if !NETCOREAPP && !NETSTANDARD2_1_OR_GREATER
#pragma warning restore CS8602
#endif
                // Write the boolean value.
                bytesWriter.Write(true);
                // Size of font config
                bytesWriter.Write(Config.Length);
                // Write the text
                bytesWriter.Write(Config);
            }
            else
            {
                // Write the boolean value.
                bytesWriter.Write(false);
            }
        }
        // Count of entries
        var count = Entries.Count;

        // Write the Count
        bytesWriter.Write(count);

        // Used to write blocks of string data.
        using var dataWriter = BytesWriter.Create(4096, Encoding.UTF8);

        // Use a dictionary to filter duplicate items.
        var textToOffset = new Dictionary<string, long>(count);

        // Write hashes and strings and filter duplicates bytes of strings.
        foreach (var pair in Entries)
        {
            // Current string
            var text = pair.Value;

            // Try to get a offset from the dictionary.
            // If false, add the current string and write it to the data block.
            if (!textToOffset.TryGetValue(text, out var offset))
            {
                // Set the offset of string bytes
                offset = dataWriter.Position;

                // Write the Text
                dataWriter.WriteStringWithEndByte(text);

                // Add the text and offset to the Dictionary
                textToOffset.Add(text, offset);
            }

            // Write the Hash
            bytesWriter.Write(RSTHash.ComputeHash(pair.Key, offset, Type));
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
    /// Indicates whether the current object is equal to another object of the same type.
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

        if (Type != other.Type || Mode != other.Mode || Entries.Count != other.Entries.Count)
        {
            return false;
        }

        foreach (var pair in Entries)
        {
            if (other.Entries.TryGetValue(pair.Key, out var value))
            {
                if (!StringComparer.Ordinal.Equals(pair.Value, value))
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
    {
        return Equals(obj as RSTFile);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        if (Version > RVersion.Ver2)
        {
            return HashCode.Combine(Version, Type, Mode, Entries);
        }
        else
        {
            return HashCode.Combine(Config, Version, Type, Mode, Entries);
        }
    }
}
