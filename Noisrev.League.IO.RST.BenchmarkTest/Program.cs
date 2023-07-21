// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System;
using System.Threading.Tasks;

using BenchmarkDotNet.Running;

namespace Noisrev.League.IO.RST.BenchmarkTest;

internal sealed class Program
{
    private static async Task<int> Main(string[] args)
    {
        BenchmarkRunner.Run(new Type[] { typeof(RSTReadTest), typeof(RSTWriteTest) });

        await Task.CompletedTask;
        return 0;
    }
}
