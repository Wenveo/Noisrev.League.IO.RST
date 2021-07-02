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
                // add
                rst.AddEntry(9527, "vicent");

                // find
                RSTEntry entry = rst.Find(9527);

                PrintEntry(entry);

                // remove
                rst.Remove(entry);

                RSTEntry entry1 = rst.Find(9527);
                Console.WriteLine($"IsNull?: {entry1 is null}");
            }

            rst.Write(File.Create(path + ".rst"), false);
            rst.Dispose();

            Console.WriteLine($"Equals: {rst.Equals(new RSTFile(File.OpenRead(@"C:\Users\Noisr\Downloads\fontconfig_en_us.txt.rst"), false, false))}");
        }
        private void PrintEntry(RSTEntry entry)
        {
            Console.WriteLine($"Hash: {entry.Hash}");
            Console.WriteLine($"Text: {entry.Text}");
            Console.WriteLine();
        }
    }
}
