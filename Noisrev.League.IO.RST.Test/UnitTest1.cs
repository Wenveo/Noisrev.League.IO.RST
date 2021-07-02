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
            RSTEntry entry = rst.Entries[0];
            Console.WriteLine(entry.Text);
            Console.WriteLine(RSTHash.ComputeHash(entry.Hash, entry.Offset, rst.Type));
            rst.Dispose();
        }
    }
}
