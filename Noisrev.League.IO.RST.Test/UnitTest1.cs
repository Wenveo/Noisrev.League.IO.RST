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

            {
                rst.AddEntry(9527, "vicent");

                RSTEntry entry = rst.Entries[rst.Entries.Count - 1];
                Console.WriteLine($"Hash: {entry.Hash}"); // 9527
                Console.WriteLine($"Text: {entry.Text}"); // vicent
            }

            rst.Write(File.Create(path + ".rst"), false);
            rst.Dispose();
        }
    }
}
