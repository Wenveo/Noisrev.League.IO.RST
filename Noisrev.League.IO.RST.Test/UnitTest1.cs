using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Noisrev.League.IO.RST.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            RSTFile rst = new RSTFile(File.OpenRead(@"C:\Users\Noisr\Downloads\fontconfig_en_us.txt"), true, false);
        }
    }
}
