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
                    ReadText();
                }
                return text;
            }
            set { text = value; }
        }
        private string text = null;
        public RSTEntry(RSTFile parent, long offset, ulong hash)
        {
            Parent = parent;
            Offset = offset;
            Hash = hash;

            if (!parent.UseLazyLoad)
            {
                ReadText();
            }
        }
        private void ReadText()
        {
            text = Parent.br.ReadStringWithEndByte(Parent.DataOffset + Offset, 0x00);
        }
    }
}
