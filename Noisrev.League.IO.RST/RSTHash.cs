using Noisrev.League.IO.RST.Helpers;
using Standart.Hash.xxHash;
using System;
using System.Text;

namespace Noisrev.League.IO.RST
{
    /// <summary>
    /// RST hashes Compute class.
    /// </summary>
    public static class RSTHash
    {
        /// <summary>
        /// Use <paramref name="toHash"/> and <paramref name="type"/> to generate a hash with no offset.
        /// </summary>
        /// <param name="toHash">The string used to generate the hash.</param>
        /// <param name="type">The type of <see cref="RSTFile"/>.</param>
        /// <returns>The generated hash.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static ulong ComputeHash(string toHash, RType type)
        {
            if (toHash == null) throw new ArgumentNullException(nameof(toHash));

            var buffer = Encoding.UTF8.GetBytes(toHash.ToLower());
            return xxHash64.ComputeHash(buffer, buffer.Length) & type.ComputeKey();
        }
        /// <summary>
        /// Generate a hash with an offset using <paramref name="toHash"/> and <paramref name="offset"/>, as well as <paramref name="type"/>.
        /// </summary>
        /// <param name="toHash">The string used to generate the hash.</param>
        /// <param name="offset">The offset of the text.</param>
        /// <param name="type">The type of <see cref="RSTFile"/>.</param>
        /// <returns>The generated hash.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static ulong ComputeHash(string toHash, long offset, RType type)
        {
            if (toHash == null) throw new ArgumentNullException(nameof(toHash));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

            var buffer = Encoding.UTF8.GetBytes(toHash.ToLower());
            return xxHash64.ComputeHash(buffer, buffer.Length) & type.ComputeKey() + offset.ComputeOffset(type);
        }
        /// <summary>
        /// Regenerate a hash with an offset using the generated <paramref name="hash"/> and <paramref name="offset"/>, as well as <paramref name="type"/>.
        /// </summary>
        /// <param name="hash">A hash used to merge offset.</param>
        /// <param name="offset">The offset of the text.</param>
        /// <param name="type">The type of <see cref="RSTFile"/>.</param>
        /// <returns>The generated hash.</returns>
        public static ulong ComputeHash(ulong hash, long offset, RType type)
        {
            return hash + offset.ComputeOffset(type);
        }
    }
}
