using Extensions.Data;
using Noisrev.League.IO.RST.Helper;
using System.Text;

namespace Noisrev.League.IO.RST
{
    /// <summary>
    /// RST hashes Compute class.
    /// </summary>
    public static class RSTHash
    {
        /// <summary>
        /// Use toHash and type to generate a hash with no offset.
        /// </summary>
        /// <param name="toHash">The toHash is used to generate a hash.</param>
        /// <param name="type">Sets the type of hash generated.</param>
        /// <returns>The generated hash.</returns>
        public static ulong ComputeHash(string toHash, RType type)
        {
            toHash = toHash.ToLower();
            return XXHash.XXH64(Encoding.UTF8.GetBytes(toHash)) & type.ComputeKey();
        }
        /// <summary>
        /// Generate a hash with an offset using toHash and offset, as well as type.
        /// </summary>
        /// <param name="toHash">The toHash is used to generate a hash.</param>
        /// <param name="offset">Set the offset to generate a hash with the offset.</param>
        /// <param name="type">Sets the type of hash generated.</param>
        /// <returns>The generated hash.</returns>
        public static ulong ComputeHash(string toHash, long offset, RType type)
        {
            return ComputeHash(toHash, type) + offset.ComputeOffset(type);
        }
        /// <summary>
        /// Regenerate a hash with an offset using the generated hash and offset, as well as type.
        /// </summary>
        /// <param name="hash">The hash that has been generated.</param>
        /// <param name="offset">Set the offset to generate a hash with the offset.</param>
        /// <param name="type">Sets the type of hash generated.</param>
        /// <returns>The generated hash.</returns>
        public static ulong ComputeHash(ulong hash, long offset, RType type)
        {
            return hash + offset.ComputeOffset(type);
        }
    }
}
