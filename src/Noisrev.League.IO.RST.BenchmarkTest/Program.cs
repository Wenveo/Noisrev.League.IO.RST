// Copyright (c) 2021 - 2023 Noisrev
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

#if NETFRAMEWORK
#nullable enable
#endif

using BenchmarkDotNet.Running;

using Noisrev.League.IO.RST.BenchmarkTest;

BenchmarkRunner.Run(new System.Type[]
{
    typeof(RSTReadTest),
    typeof(RSTWriteTest),
    typeof(RSTHashTest)
});
