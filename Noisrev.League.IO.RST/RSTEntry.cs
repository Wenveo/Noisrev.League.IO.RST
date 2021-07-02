using System;

namespace Noisrev.League.IO.RST
{
    /// <summary>
    /// This is the content entry for the RSTFile.
    /// </summary>
    public class RSTEntry : IEquatable<RSTEntry>
    {
        /// <summary>
        /// The parent class
        /// </summary>
        public RSTFile Parent { get; }
        /// <summary>
        /// Data offset, used to set the specified stream position and read the content
        /// </summary>
        public long Offset { get; set; }
        /// <summary>
        /// The hash located at Entry
        /// </summary>
        public ulong Hash { get; set; }
        /// <summary>
        /// The content of the entry. String type
        /// </summary>
        public string Text
        {
            get
            {
                if (text is null)
                {
                    Parent.ReadText(this);
                }
                return text;
            }
            set { text = value; }
        }
        private string text = null;
        /// <summary>
        /// Initialize RSTEntry, and set the parent, offset, and hash.
        /// </summary>
        /// <param name="parent">RST File class</param>
        /// <param name="offset">Data offset</param>
        /// <param name="hash">The hash</param>
        public RSTEntry(RSTFile parent, long offset, ulong hash)
        {
            Parent = parent;
            Offset = offset;
            Hash = hash;
        }

        public bool Equals(RSTEntry other)
        {
            if (other is null)
            {
                return false;
            }
            else
            {
                return Hash == other.Hash
                    && Text == other.Text;
            }
        }
    }
}
