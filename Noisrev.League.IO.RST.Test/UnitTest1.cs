using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Noisrev.League.IO.RST.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string path = @"C:\Users\Noisr\Downloads\fontconfig_en_us.txt";
            RSTFile rst = new(
                input: File.OpenRead(path),
                leaveOpen: false,
                useLazyLoad: true);

            rst.Write(File.Create(path + ".rst"), false);
            rst.Dispose();
        }
    }
}
