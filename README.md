# 📖 Noisrev.League.IO.RST

***This is a library for parsing and editing RST files of League of Legends.***

[![NetStandard](https://img.shields.io/badge/.Net%20Standard-v2.0-brightgreen)](https://learn.microsoft.com/dotnet/standard/net-standard?tabs=net-standard-2-0) [![LICENSE](https://img.shields.io/github/license/noisrev/noisrev.league.io.rst)](https://opensource.org/licenses/MIT) [![Downloads](https://img.shields.io/nuget/dt/noisrev.league.io.rst)](https://www.nuget.org/packages/Noisrev.League.IO.RST) [![PackageVersion](https://img.shields.io/nuget/vpre/noisrev.league.io.rst)](https://www.nuget.org/packages/Noisrev.League.IO.RST/#versions-body-tab)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FNoisrev%2FNoisrev.League.IO.RST.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FNoisrev%2FNoisrev.League.IO.RST?ref=badge_shield)

[![NetFramework](https://img.shields.io/badge/.Net%20Framework->=4.6.2-green)](https://github.com/microsoft/dotnet/tree/main/releases/net462) [![NetCore](https://img.shields.io/badge/.Net%20Core->=v6.0-blue)](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-6) [![CodeQL](https://github.com/Noisrev/Noisrev.League.IO.RST/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Noisrev/Noisrev.League.IO.RST/actions/workflows/codeql-analysis.yml) [![.NET](https://github.com/Noisrev/Noisrev.League.IO.RST/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Noisrev/Noisrev.League.IO.RST/actions/workflows/dotnet.yml)

[![Bilibili](https://img.shields.io/badge/dynamic/json?color=ff69b4&label=bilibili&query=%24.data.totalSubs&url=https%3A%2F%2Fapi.spencerwoo.com%2Fsubstats%2F%3Fsource%3Dbilibili%26queryKey%3D176863848)](https://space.bilibili.com/176863848)

## 🔥 Version 4 - Preview!
Improved performance for Read/Write, Less time/op and lower memory allocation. ^^

| Method |              Runtime |      Mean | Allocated |
|------- |--------------------- |----------:|----------:|
| (Version 4)
|   Open |             .NET 8.0 |  56.04 ms |     24 MB |
|  Write |             .NET 8.0 |  38.48 ms |     28 MB |
| (Version 3)
|   Open |             .NET 6.0 |  74.13 ms |     36 MB |
|  Write |             .NET 6.0 |  52.20 ms |     30 MB |


## ✨ RST (Riot String Table)
The file is used to store strings of in-game text.
- Champion name, skill description, skin name, etc...
- *Like:* **"Riven"**, **"Championship Riven 2016"**, **"\<mainText>\<stats>\<attention>%i:scaleAP% 25\</attention> Ability Power\<br>\<attention>%i:scaleMPen% 15%\</attention> Magic Penetration\</stats>\</mainText>\<br>"**
- And more...

***If you would like to see more details about the RST file, please see: [RSTFile.cs](src/Noisrev.League.IO.RST/RSTFile.cs).***

## 🔖 Getting Started

1. Use the NuGet Libaray
>- Install via **NuGet Package Manager** in **Visual Studio**
>- .NET CLI : `dotnet add package Noisrev.League.IO.RST`

2. Then load a RSTFile using:
``` C#
using Noisrev.League.IO.RST;

var rst = RSTFile.Load("your rst file path");
```

For more information, refer to [Getting Started](https://wenveo.github.io/Noisrev.League.IO.RST/index.html).

## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FNoisrev%2FNoisrev.League.IO.RST.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FNoisrev%2FNoisrev.League.IO.RST?ref=badge_large)
