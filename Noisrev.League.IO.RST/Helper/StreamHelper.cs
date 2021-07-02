using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Noisrev.League.IO.RST.Helper
{
    /// <summary>
    /// Stream extension classes.
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        /// Loop through the bytes and stop reading when a matching <paramref name="end"/> is read.
        /// </summary>
        /// <param name="input">BinaryReader</param>
        /// <param name="end">End Byte</param>
        /// <returns>UTF-8 string</returns>
        public static string ReadStringWithEndByte<T>(this T input, long offset, byte end) where T : Stream
        {
            // Set Offset
            input.Seek(offset, SeekOrigin.Begin);
            // Bytes Buffer
            List<byte> Buffer = new List<byte>();

            // temp byte
            byte tmp;
            // Loop byte read
            while (/*input.CanRead && */(tmp = (byte)input.ReadByte()) != end)
            {
                // Current byte is not end byte, added to buffer
                Buffer.Add(tmp);
            }
            // To an array and convert it to a UTF-8 string
            return Buffer.ToArray().GetString(Encoding.UTF8);
        }
        /// <summary>
        /// Copy the stream and return the starting position.
        /// </summary>
        /// <param name="src">left</param>
        /// <param name="dst">right</param>
        public static void AutoCopy(this Stream src, out Stream dst)
        {
            // Init MemoryStream
            dst = new MemoryStream();
            // Set Start
            long start = src.Position;
            // Copy the remaining contents of the buffer to the DataStream.
            src.CopyTo(dst);
            // Back To Start
            src.Seek(start, SeekOrigin.Begin);
        }
    }
}
