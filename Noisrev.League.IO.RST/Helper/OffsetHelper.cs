﻿using System;

namespace Noisrev.League.IO.RST.Helper
{
    /// <summary>
    ///     Offset extension class.
    /// </summary>
    public static class OffsetHelper
    {
        /// <summary>
        ///     Use <see cref="RType" /> to generate offsets for <see cref="RSTEntry" />
        /// </summary>
        /// <param name="offset">The offset</param>
        /// <param name="type">The type</param>
        /// <returns>Returns the generated offset.</returns>
        /// <exception cref="OverflowException" />
        public static ulong ComputeOffset(this long offset, RType type)
        {
            return Convert.ToUInt64(offset << (byte) type);
        }
    }
}