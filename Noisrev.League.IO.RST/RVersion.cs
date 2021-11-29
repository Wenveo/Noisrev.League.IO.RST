using System;
using System.Collections.Generic;
using System.Text;

namespace Noisrev.League.IO.RST
{
    /// <summary>
    /// RST File Version
    /// </summary>
    public enum RVersion : byte
    {
        /// <summary>
        /// Version 2
        /// </summary>
        Ver2 = 0x02,
        /// <summary>
        /// Version 3
        /// </summary>
        Ver3 = 0x03,
        /// <summary>
        /// Version 4
        /// </summary>
        Ver4 = 0x04,
        /// <summary>
        /// Version 5
        /// </summary>
        Ver5 = 0x05
    }
}
