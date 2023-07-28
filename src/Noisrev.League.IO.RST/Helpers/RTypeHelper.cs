// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.Runtime.CompilerServices;

namespace Noisrev.League.IO.RST.Helpers;

/// <summary>
/// RType extension class.
/// </summary>
public static class RTypeHelper
{
    /// <summary>
    /// Compute the key used to generate the hash.
    /// </summary>
    /// <param name="type">The type of <see cref="RSTFile"/>.</param>
    /// <returns>The computed value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ComputeKey(this RType type)
    {
        return (1UL << (int)type) - 1;
    }

    /// <summary>
    /// Gets the specified <see cref="RType"/> based on <see cref="RVersion"/>.
    /// </summary>
    /// <param name="version">The version of <see cref="RSTFile"/>.</param>
    /// <returns>A <see cref="RType"/>, or null, depending on whether it is a valid <see cref="RVersion"/>.</returns>
    public static RType? GetRType(this RVersion version)
    {
        /* Version 2 and Version 3 */
        if (version is RVersion.Ver2 or RVersion.Ver3)
            return RType.Complex;
        else if (version is RVersion.Ver4 or RVersion.Ver5) /* Version 4, 5 */
            return RType.Simple;
        else /* Unknown */
            return null;
    }
}
