// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

#if NETFRAMEWORK
#nullable enable
#endif

using System.IO;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Noisrev.League.IO.RST.Test;

[SimpleJob(RuntimeMoniker.Net462)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[RPlotExporter]
[RankColumn]
public class RSTReadTest
{
    private string _filePath = null!;

    [GlobalSetup]
    public void Setup()
    {
        _filePath = Path.Combine("Resources", "fontconfig_en_us.txt");
    }

    [Benchmark]
    public void Read()
    {
        _ = new RSTFile(File.OpenRead(_filePath), false);
    }
}
