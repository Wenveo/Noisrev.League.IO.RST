using Noisrev.League.IO.RST.Helper;

namespace Noisrev.League.IO.RST
{
    public class RSTEntry
    {
        public RSTFile Parent { get; }
        public long Offset { get; set; }
        public ulong Hash { get; set; }
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
        public string text = null;
        public RSTEntry(RSTFile parent, long offset, ulong hash)
        {
            Parent = parent;
            Offset = offset;
            Hash = hash;
        }
    }
}
