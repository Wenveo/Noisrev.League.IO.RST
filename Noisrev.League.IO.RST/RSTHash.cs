// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO.Hashing;
using System.Text;

using Noisrev.League.IO.RST.Helpers;

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
        unsafe
        {
            var length = toHash.Length;
            var ch = stackalloc char[length];

#if NET7_0_OR_GREATER
            MemoryExtensions.ToLower(
                new ReadOnlySpan<char>(in toHash.GetPinnableReference()),
                new Span<char>(ch, length), CultureInfo.CurrentCulture);
#elif NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            MemoryExtensions.ToLower(toHash.AsSpan(), new(ch, length), CultureInfo.CurrentCulture);
#else
            // This code was tested to perform better than string.ToLower on .Net Framework
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            for (var i = 0; i < length; i++)
            {
                ch[i] = textInfo.ToLower(toHash[i]);
            }
#endif

            var byteCount = Encoding.UTF8.GetByteCount(ch, length);
            var bytes = stackalloc byte[byteCount];

            var bytesReceived = Encoding.UTF8.GetBytes(ch, length, bytes, byteCount);
            Debug.Assert(byteCount == bytesReceived);

            return XxHash64.HashToUInt64(new ReadOnlySpan<byte>(bytes, bytesReceived)) & type.ComputeKey();
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
