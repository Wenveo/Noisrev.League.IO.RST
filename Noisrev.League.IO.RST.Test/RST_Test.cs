using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net.Http;
using System;
using System.Diagnostics;

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
                string uri = "https://raw.communitydragon.org/latest/game/data/menu/fontconfig_en_us.txt";
                HttpClient http = new HttpClient();
                var result = http.GetAsync(uri).Result;
                if (result.IsSuccessStatusCode)
                {
                    using (FileStream fs = File.Create(en_us))
                    {
                        using (Stream buffer = result.Content.ReadAsStream())
                        {
                            buffer.CopyTo(fs);
                        }
                    }
                }
            }
        }
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
        public void _Equals()
        {
            RSTFile left = new RSTFile(File.OpenRead(en_us), false);
            RSTFile right = new RSTFile(File.OpenRead(output), false);

            Assert.IsTrue(left.Equals(right));
        }

        static RSTFile _TEST = new RSTFile(RVersion.Ver5);

        [TestMethod]
        public void AddTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            {
                for (int i = 0; i < 10000000; i++)
                {
                    _TEST.Add((ulong)i, i.ToString());
                }
            }

            watch.Stop();
            TimeSpan timespan = watch.Elapsed;
            Console.WriteLine("watch: {0}(ms)", timespan.TotalMilliseconds);
        }
        [TestMethod]
        public void RemoveTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            {
                foreach (var item in _TEST)
                {
                    _TEST.Remove(item.Key);
                }
            }

            watch.Stop();
            TimeSpan timespan = watch.Elapsed;
            Console.WriteLine("watch: {0}(ms)", timespan.TotalMilliseconds);
        }
    }
}
