# Getting Started

**Noisrev.League.IO.RST** is a powerful parsing library for RST files, in **.NET**. It provides fast parsing, editing and writing of RST files.

## Quick Start

- Install via **NuGet Package Manager** in **Visual Studio**
- .NET CLI : `dotnet add package Noisrev.League.IO.RST`

``` C#
using Noisrev.League.IO.RST;

var rst = RSTFile.Load("your rst file path");
```

## Build RST files with RSTBuilder

**RSTBuilder** provides some methods to easily editing the entries of **RSTFile**.

Example:

``` cs
// Create a new RSTBuilder, with a local file
var builder = new RSTBuilder(RSTFile.Load("your rst file path"));
// Or with a new RSTFile
// var builder = new RSTBuilder(new RSTFile(RVersion.Ver5));

// Add some elements to the current RSTFile.
builder.Add(56281743, "TestValue");
builder.Add(RSTHash.ComputeHash("ItemA", RType.Simple), "Some Text.");

// Replace all matching strings in the current RSTFile.
builder.ReplaceAll("Miss Fortune", "Uwu MF");

// And then save the current RSTFile.
builder.Build("The output file path.");
```
