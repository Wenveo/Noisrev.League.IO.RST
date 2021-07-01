using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Noisrev.League.IO.RST.Helper
{
    /// <summary>
    /// Binary Reader operation class
    /// </summary>
    public static class BinaryReaderHelper
    {
        /// <summary>
        /// Use BinaryReader to read the number(<paramref name="count"/>) of bytes.
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <param name="count">size or Length</param>
        /// <returns>UTF-8 string</returns>
        public static string ReadString(this BinaryReader br, int count)
        {
            // Read count bytes and return a UTF-8 string
            return br.ReadBytes(count).GetString(Encoding.UTF8);
        }
        /// <summary>
        /// Loop through the bytes and stop reading when a matching <paramref name="end"/> is read.
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <param name="end">End Byte</param>
        /// <returns>UTF-8 string</returns>
        public static string ReadStringWithEndByte<T>(this T br, long offset, byte end) where T : BinaryReader
        {
            // Save the current stream Position
            long start = br.BaseStream.Position;
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            // Bytes Buffer
            List<byte> Buffer = new List<byte>();

            // temp byte
            byte tmp;
            // Loop byte read
            while (/*br.BaseStream.CanRead && */(tmp = br.ReadByte()) != end)
            {
                // Current byte is not end byte, added to buffer
                Buffer.Add(tmp);
            }
            // Back to Start
            br.BaseStream.Seek(start, SeekOrigin.Begin);
            // To an array and convert it to a UTF-8 string
            return Buffer.ToArray().GetString(Encoding.UTF8);
        }
    }
}
