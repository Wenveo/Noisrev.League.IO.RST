using System;
using System.Text;

namespace Noisrev.League.IO.RST.Helper
{
    /// <summary>
    /// Byte array extension class.
    /// </summary>
    public static class BytesHelper
    {
        /// <summary>
        /// Return the string of the encoding specified in encoding.
        /// </summary>
        /// <param name="buffer">Byte array</param>
        /// <param name="encoding">The encoding</param>
        /// <returns>A string that specifies the character encoding.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DecoderExceptionFallback"></exception>
        public static string GetString(this byte[] buffer, Encoding encoding)
        {
            // Return String
            return encoding.GetString(buffer);
        }
    }
}
