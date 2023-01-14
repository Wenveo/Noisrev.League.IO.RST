/*
 * Copyright (c) 2021 - 2023 Noisrev
 * All rights reserved.
 *
 * This source code is distributed under an MIT license. 
 * LICENSE file in the root directory of this source tree.
 */

using System;

namespace Noisrev.League.IO.RST.Helpers;

/// <summary>
/// Offset extension class.
/// </summary>
public static class OffsetHelper
{
    /// <summary>
    /// Use <see cref="RType"/> to generate offsets for RST elements.
    /// </summary>
    /// <param name="offset">The offset</param>
    /// <param name="type">The type</param>
    /// <returns>Returns the generated offset.</returns>
    /// <exception cref="OverflowException"/>
    public static ulong ComputeOffset(this long offset, RType type)
    {
        return Convert.ToUInt64(offset << (byte)type);
    }
}
