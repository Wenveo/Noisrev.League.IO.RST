// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.IO.Hashing;
using System.Text;

using Noisrev.League.IO.RST.Helpers;
using Noisrev.League.IO.RST.Internal;

namespace Noisrev.League.IO.RST;

/// <summary>
/// RST hashes compute class.
/// </summary>
public static class RSTHash
{
    /// <summary>
    /// Generate a hash without offset based on the <see cref="string"/> and <see cref="RType"/>.
    /// </summary>
    /// <param name="toHash">The string used to generate the hash.</param>
    /// <param name="type">The type of <see cref="RSTFile"/>.</param>
    /// <returns>The generated hash.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static ulong ComputeHash(string toHash, RType type)
    {
        return ComputeHash(toHash, type, Encoding.UTF8, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Generate a hash without offset based on the <see cref="string"/> and <see cref="RType"/>.
    /// </summary>
    /// <param name="toHash">The string used to generate the hash.</param>
    /// <param name="type">The type of <see cref="RSTFile"/>.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <returns>The generated hash.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static ulong ComputeHash(string toHash, RType type, Encoding encoding)
    {
        return ComputeHash(toHash, type, encoding, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Generate a hash without offset based on the <see cref="string"/> and <see cref="RType"/>.
    /// </summary>
    /// <param name="toHash">The string used to generate the hash.</param>
    /// <param name="type">The type of <see cref="RSTFile"/>.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="cultureInfo">An object that supplies culture-specific casing rules.</param>
    /// <returns>The generated hash.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static ulong ComputeHash(string toHash, RType type, Encoding encoding, CultureInfo cultureInfo)
    {
        unsafe
        {
            var length = toHash.Length;
            if (length <= 1024)
            {
                var ch = stackalloc char[length];
                Utilities.ToLower(toHash.AsSpan(), new(ch, length), cultureInfo);
                return ComputeHashCore(ch, length, type, encoding);
            }

            var charArray = ArrayPool<char>.Shared.Rent(length);
            try
            {
                fixed (char* ch = charArray)
                {
                    Utilities.ToLower(toHash.AsSpan(), new(ch, length), cultureInfo);
                    return ComputeHashCore(ch, length, type, encoding);
                }
            }
            finally
            {
                ArrayPool<char>.Shared.Return(charArray);
            }

            static ulong ComputeHashCore(char* ch, int length, RType type, Encoding encoding)
            {
                var byteCount = encoding.GetByteCount(ch, length);
                if (byteCount <= 1024)
                {
                    var bytes = stackalloc byte[byteCount];
                    var bytesReceived = encoding.GetBytes(ch, length, bytes, byteCount);
                    Debug.Assert(byteCount <= bytesReceived);

                    return XxHash64.HashToUInt64(new ReadOnlySpan<byte>(bytes, bytesReceived)) & type.ComputeKey();
                }

                var tempBuffer = ArrayPool<byte>.Shared.Rent(byteCount);
                try
                {
                    fixed (byte* bytes = tempBuffer)
                    {
                        var bytesReceived = encoding.GetBytes(ch, length, bytes, byteCount);
                        Debug.Assert(byteCount <= bytesReceived);

                        return XxHash64.HashToUInt64(new ReadOnlySpan<byte>(bytes, bytesReceived)) & type.ComputeKey();
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(tempBuffer);
                }
            }

        }
    }

    /// <summary>
    /// Generate a hash based on the <see cref="string"/> and <paramref name="offset"/> and <see cref="RType"/>.
    /// </summary>
    /// <param name="toHash">The string used to generate the hash.</param>
    /// <param name="offset">The offset of the text.</param>
    /// <param name="type">The type of <see cref="RSTFile"/>.</param>
    /// <returns>The generated hash.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public static ulong ComputeHash(string toHash, long offset, RType type)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
#else
        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }
#endif
        return ComputeHash(toHash, type) + offset.ComputeOffset(type);
    }

    /// <summary>
    /// Regenerate a new hash with <paramref name="offset"/> using the generated <paramref name="hash"/> and <paramref name="type"/>.
    /// </summary>
    /// <param name="hash">A hash used to merge offset.</param>
    /// <param name="offset">The offset of the text.</param>
    /// <param name="type">The type of <see cref="RSTFile"/>.</param>
    /// <returns>The regenerated hash with offset</returns>
    public static ulong ComputeHash(ulong hash, long offset, RType type)
    {
        return hash + offset.ComputeOffset(type);
    }
}
