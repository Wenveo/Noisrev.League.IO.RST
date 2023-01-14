/*
 * Copyright (c) 2021 - 2023 Noisrev
 * All rights reserved.
 *
 * This source code is distributed under an MIT license. 
 * LICENSE file in the root directory of this source tree.
 */

namespace Noisrev.League.IO.RST;

/// <summary>
/// RST File Type
/// </summary>
public enum RType
{
    /// <summary>
    /// using by RST v2, v3
    /// </summary>
    Complex = 40,
    /// <summary>
    /// using by RST v4, v5
    /// </summary>
    Simple = 39
}
