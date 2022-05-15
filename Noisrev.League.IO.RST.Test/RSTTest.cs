using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.IO;

namespace Noisrev.League.IO.RST.Test
{
    public class Program
    {
        [SimpleJob(RuntimeMoniker.Net472)]
        [SimpleJob(RuntimeMoniker.NetCoreApp31)]
        [SimpleJob(RuntimeMoniker.Net50)]
        [SimpleJob(RuntimeMoniker.Net60)]
        [MemoryDiagnoser]
        [RPlotExporter]
        [RankColumn]
        public class RSTTest
        {
            public static readonly string en_us = Path.Combine("Resources", "fontconfig_en_us.txt");
            public static readonly string output = Path.Combine("Resources", "fontconfig_en_us.txt.rst");
            public static RSTFile InputRSTFile;

            [GlobalSetup]
            public void Setup()
            {
                InputRSTFile = new RSTFile(File.OpenRead(en_us), false);
            }


            [Benchmark]
            public void Open()
            {
                InputRSTFile = new RSTFile(File.OpenRead(en_us), false);
            }

            [Benchmark]
            public void Write()
            {
                InputRSTFile.Write(File.Create(output), false);
            }
        }

        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<RSTTest>();
            //RSTTest.InputRSTFile = new RSTFile(File.OpenRead(RSTTest.en_us), false);
        }
    }
}
