/*
 * Copyright (c) 2021 - 2023 Noisrev
 * All rights reserved.
 *
 * This source code is distributed under an MIT license. 
 * LICENSE file in the root directory of this source tree.
 */

using System.IO;

namespace Noisrev.League.IO.RST.Helpers
{
    /// <summary>
    /// Stream extension class.
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        /// Reads from the current position of the <paramref name="stream"/> to the end, and returns an array of bytes.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <returns>An array of bytes that have been read from the stream.</returns>
        /// <exception cref="EndOfStreamException"></exception>
        public static byte[] ReadToEnd(this Stream stream)
        {
            var position = stream.Position;
            var rentSize = (int)(stream.Length - position);

            // Maybe use ArrayPool<byte>.Shared.Rent ?
            var buffer = new byte[rentSize];
            var result = stream.Read(buffer, 0, rentSize);
            if (result != rentSize)
                throw new EndOfStreamException();

            return buffer;
        }
    }
}