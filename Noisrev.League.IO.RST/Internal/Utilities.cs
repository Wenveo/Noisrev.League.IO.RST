
// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Noisrev.League.IO.RST.Internal;

internal static class Utilities
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToLower(ReadOnlySpan<char> source, Span<char> destination, CultureInfo cultureInfo)
    {
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
        source.ToLower(destination, cultureInfo);
#else
        // This code was tested to perform better than string.ToLower on .Net Framework
        var textInfo = cultureInfo.TextInfo;
        for (var i = 0; i < source.Length; i++)
        {
            destination[i] = textInfo.ToLower(source[i]);
        }
#endif
    }
}
