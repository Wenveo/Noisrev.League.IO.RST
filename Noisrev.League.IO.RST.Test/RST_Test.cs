using System.IO;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Noisrev.League.IO.RST.Test
{
    [TestClass]
    public class RST_Test
    {
        private readonly string en_us = "fontconfig_en_us.txt";
        private readonly string output = "fontconfig_en_us.txt.rst";

        public RST_Test()
        {
            if (!File.Exists(en_us))
            {
                var uri = "https://raw.communitydragon.org/latest/game/data/menu/fontconfig_en_us.txt";
                var http = new HttpClient();
                var result = http.GetAsync(uri).Result;
                if (result.IsSuccessStatusCode)
                    using (var fs = File.Create(en_us))
                    {
                        using (var buffer = result.Content.ReadAsStream())
                        {
                            buffer.CopyTo(fs);
                        }
                    }
            }
        }

        [TestMethod]
        public void A_Open()
        {
            _ = new RSTFile(File.OpenRead(en_us), false);
        }

        [TestMethod]
        public void B_OpenWrite()
        {
            var rst = new RSTFile(File.OpenRead(en_us), false);
            rst.Write(File.Create(output), false);
        }

        [TestMethod]
        public void C_Equals()
        {
            var left = new RSTFile(File.OpenRead(en_us), false);
            var right = new RSTFile(File.OpenRead(output), false);

            Assert.IsTrue(left.Equals(right));
        }

        [TestMethod]
        public void __R()
        {
            var path = @"C:\Users\Noisr\Desktop\fontconfig_zh_cn.txt";

            var rst = new RSTFile(File.OpenRead(path), false);

            rst.Write(File.Create(path + ".rst"), false);
        }
    }
}