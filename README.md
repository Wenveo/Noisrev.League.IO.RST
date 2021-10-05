# Noisrev.League.IO.RST

***This is a library that parses and manipulates League of Legends RST files.***

<a>
  <img src="https://img.shields.io/badge/.Net%20Standard-v2.1-brightgreen"></img>
</a>
<a href="https://github.com/Noisrev/Noisrev.League.IO.RST/blob/master/LICENSE/">
  <img src="https://img.shields.io/github/license/noisrev/noisrev.league.io.rst"></img>
</a>
<a href="https://www.nuget.org/packages/Noisrev.League.IO.RST/">
  <img src="https://img.shields.io/nuget/dt/noisrev.league.io.rst"></img>
</a>
<a href="https://www.nuget.org/packages/Noisrev.League.IO.RST/">
  <img src="https://img.shields.io/nuget/v/noisrev.league.io.rst"></img>
</a>

# RST (Riot String Table)
The RST file is a file that stores a list of strings.

It is used to store text messages in League of Legends games.
- ***For example:*** champion name, skill description, skin name, etc
- *Like:* **"Riven"**, **"Championship Riven 2016"**, **"\<mainText>\<stats>\<attention>%i:scaleAP% 25\</attention> Ability Power\<br>\<attention>%i:scaleMPen% 15%\</attention> Magic Penetration\</stats>\</mainText>\<br>"**
- And more...

***If you would like to see more details about the RST file, please see: [RSTFile.cs](Noisrev.League.IO.RST/RSTFile.cs).***


# Release Note

***v1.3.1***
- Change the get method for "DataOffset"

***v1.3.0***
- Support RST v5
- Fix some code
- Correct some code comments

***v1.2.1***
- Remove unnecessary code.

***v1.2***
- Remove lazy loading mode.
- Add the "SetConfig" method. // This is only applicable to RST files in version 2.1

***v1.1***
- Add "Insert" method.

***v1.0***
- Support for reading and writing RST.
- Support loading RST files using lazy loading mode.
- Add" Add", "Find", "Remove", "ReplaceAll" methods.
- Add "RST Hash Algorithm".

# Install
- Install using NuGet Package Manager in Visual Studio.
- .NET CLI : `dotnet add package Noisrev.League.IO.RST`

# Simple
```
using Noisrev.League.IO.RST;

RSTFile rst = new(input: File.OpenRead("your rst file path"), leaveOpen: false);
```
