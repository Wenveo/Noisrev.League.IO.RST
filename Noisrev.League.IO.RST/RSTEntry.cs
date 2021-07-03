using System;
using System.IO;
using System.Text;

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
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="DecoderExceptionFallback"></exception>
        public string Text
        {
            get
            {
                // Lazy Loading
                if (text is null)
                {
                    // Get Content
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
        /// <param name="parent">RST File</param>
        /// <param name="offset">Data offset</param>
        /// <param name="hash">The hash</param>
        public RSTEntry(RSTFile parent, long offset, ulong hash)
        {
            Parent = parent;
            Offset = offset;
            Hash = hash;
        }
        /// <summary>
        /// Initialize RSTEntry, and set the parent, hash, and value.
        /// </summary>
        /// <param name="parent">RST File</param>
        /// <param name="hash">The hash</param>
        /// <param name="value">The content</param>
        public RSTEntry(RSTFile parent, ulong hash, string value) : this(parent, 0, hash)
        {
            text = value;
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
