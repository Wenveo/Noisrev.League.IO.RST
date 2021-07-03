using System;
using System.IO;
using System.Text;

namespace Noisrev.League.IO.RST.Helper
{
    /// <summary>
    /// Binary Reader extension class.
    /// </summary>
    public static class BinaryReaderHelper
    {
        /// <summary>
        /// Use BinaryReader to read the number(<paramref name="count"/>) of bytes.
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <param name="count">size or Length</param>
        /// <returns>UTF-8 string</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="DecoderExceptionFallback"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="IOException"></exception>
        public static string ReadString<T>(this T br, int count) where T : BinaryReader
        {
            // Read count bytes and return a UTF-8 string
            return br.ReadBytes(count).GetString(Encoding.UTF8);
        }
    }
}
