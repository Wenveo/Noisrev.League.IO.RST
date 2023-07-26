# 📖 Noisrev.League.IO.RST

***This is a library for parsing and editing League of Legends RST files.***

![NetStandard](https://img.shields.io/badge/.Net%20Standard-v2.0-brightgreen) ![LICENSE](https://img.shields.io/github/license/noisrev/noisrev.league.io.rst) ![Downloads](https://img.shields.io/nuget/dt/noisrev.league.io.rst) ![PackageVersion](https://img.shields.io/nuget/v/noisrev.league.io.rst)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FNoisrev%2FNoisrev.League.IO.RST.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FNoisrev%2FNoisrev.League.IO.RST?ref=badge_shield)

![NetFramework](https://img.shields.io/badge/.Net%20Framework->=4.6.1-green) ![NetCore](https://img.shields.io/badge/.Net%20Core->=v2.0-blue) [![CodeQL](https://github.com/Noisrev/Noisrev.League.IO.RST/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Noisrev/Noisrev.League.IO.RST/actions/workflows/codeql-analysis.yml) [![.NET](https://github.com/Noisrev/Noisrev.League.IO.RST/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Noisrev/Noisrev.League.IO.RST/actions/workflows/dotnet.yml)

[![Bilibili](https://img.shields.io/badge/dynamic/json?color=ff69b4&label=bilibili&query=%24.data.totalSubs&url=https%3A%2F%2Fapi.spencerwoo.com%2Fsubstats%2F%3Fsource%3Dbilibili%26queryKey%3D176863848)](https://space.bilibili.com/176863848)

# ✨ RST (Riot String Table) 
This file is used to store strings of in-game text.
- champion name, skill description, skin name, etc...
- *Like:* **"Riven"**, **"Championship Riven 2016"**, **"\<mainText>\<stats>\<attention>%i:scaleAP% 25\</attention> Ability Power\<br>\<attention>%i:scaleMPen% 15%\</attention> Magic Penetration\</stats>\</mainText>\<br>"**
- And more...

***If you would like to see more details about the RST file, please see: [RSTFile.cs](Noisrev.League.IO.RST/RSTFile.cs).***

# 🚀 Install
- Install via **Nuget Package Manager** in **Visual Studio**
- .NET CLI : `dotnet add package Noisrev.League.IO.RST`

# 💡 Quick Start
```
using Noisrev.League.IO.RST;

var rst = RSTFile.Load("your rst file path");
```


## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FNoisrev%2FNoisrev.League.IO.RST.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FNoisrev%2FNoisrev.League.IO.RST?ref=badge_large)
