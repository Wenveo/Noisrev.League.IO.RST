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
/// RVersion helper class.
/// </summary>
public static class RVersionHelper
{
    /// <summary>
    /// Get Latest Version of RST
    /// </summary>
    /// <returns></returns>
    public static RVersion GetLatestVersion()
    {
        var array = Enum.GetValues(typeof(RVersion));
        return (RVersion)array.GetValue(array.Length - 1)!;
    }
}
