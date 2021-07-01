using System;
using System.Collections.Generic;
using System.Text;

namespace Noisrev.League.IO.RST.Helper
{
    public static class BytesHelper
    {
        public static string GetString(this byte[] buffer, Encoding encoding)
        {
            return encoding.GetString(buffer);
        }
    }
}
