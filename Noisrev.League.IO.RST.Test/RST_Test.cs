using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;

namespace Noisrev.League.IO.RST.Test
{
    [TestClass]
    public class RST_Test
    {
        private readonly string en_us = @"C:\Users\Noisr\Downloads\fontconfig_en_us.txt";
        private readonly string output = @"C:\Users\Noisr\Downloads\fontconfig_en_us.txt.rst";

        [TestMethod]
        public void Open()
        {
            _ = new RSTFile(File.OpenRead(en_us), false);
        }
        [TestMethod]
        public void OpenWrite()
        {
            RSTFile rst = new RSTFile(File.OpenRead(en_us), false);
            rst.Write(File.Create(output), false);
        }
        [TestMethod]
        public void Equals()
        {
            RSTFile left = new RSTFile(File.OpenRead(en_us), false);
            RSTFile right = new RSTFile(File.OpenRead(output), false);

            Assert.IsTrue(left.Equals(right));
        }
    }
}
