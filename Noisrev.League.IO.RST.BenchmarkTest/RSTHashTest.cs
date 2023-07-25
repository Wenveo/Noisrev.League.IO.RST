// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

#if NETFRAMEWORK
#nullable enable
#endif

using System;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Noisrev.League.IO.RST.BenchmarkTest;

[SimpleJob(RuntimeMoniker.Net462)]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser, RPlotExporter]
[RankColumn, MValueColumn]
public class RSTHashTest
{
    private string _toHash = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(69);

        var ch = new char[32];
        for (var i = 0; i < 32; i++)
        {
            ch[i] = (char)(random.Next(1, 26) + 64);
        }

        _toHash = new string(ch);
    }

    [Benchmark]
    public void ComputeHash()
    {
        RSTHash.ComputeHash(_toHash, RType.Simple);
    }
}

