using System;

namespace Noisrev.League.IO.RST.Helper
{
    public static class OffsetHelper
    {
        public static ulong ComputeOffset(this long offset, RType type)
        {
            return Convert.ToUInt64(offset << (int)(type));
        }
    }
}
