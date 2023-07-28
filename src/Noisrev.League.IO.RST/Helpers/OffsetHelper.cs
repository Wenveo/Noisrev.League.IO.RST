// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Runtime.CompilerServices;

namespace Noisrev.League.IO.RST.Helpers;

/// <summary>
/// Offset extension class.
/// </summary>
public static class OffsetHelper
{
    /// <summary>
    /// Use <see cref="RType"/> to generate offsets for that text of <see cref="RSTFile"/>.
    /// </summary>
    /// <param name="offset">The offset of that text.</param>
    /// <param name="type">The type of that <see cref="RSTFile"/>.</param>
    /// <returns>The generated offset.</returns>
    /// <exception cref="OverflowException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ComputeOffset(this long offset, RType type)
    {
        return (ulong)offset << (byte)type;
    }
}
