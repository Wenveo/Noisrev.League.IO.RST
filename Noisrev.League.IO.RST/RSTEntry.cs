using System;

namespace Noisrev.League.IO.RST
{
    /// <summary>
    ///     This is the content entry for the RSTFile.
    /// </summary>
    public class RSTEntry : ICloneable, IEquatable<RSTEntry>
    {
        /// <summary>
        ///     Initialize RSTEntry, and set the parent, offset, and hash.
        /// </summary>
        /// <param name="offset">Data offset</param>
        /// <param name="hash">The hash</param>
        public RSTEntry(long offset, ulong hash)
        {
            Offset = offset;
            Hash = hash;
        }

        /// <summary>
        ///     Initialize RSTEntry, and set the hash, value.
        /// </summary>
        /// <param name="hash">The hash</param>
        /// <param name="value">The content</param>
        public RSTEntry(ulong hash, string value) : this(0, hash)
        {
            Text = value;
        }

        /// <summary>
        ///     Data offset, used to set the specified stream position and read the content
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        ///     The hash located at Entry
        /// </summary>
        public ulong Hash { get; set; }

        /// <summary>
        ///     The content of the entry. String type
        /// </summary>
        public string Text { get; set; }

        public object Clone()
        {
            return new RSTEntry(Hash, Text);
        }

        public bool Equals(RSTEntry other)
        {
            if (other is null)
                return false;
            return Hash == other.Hash
                   && Text == other.Text;
        }
    }
}