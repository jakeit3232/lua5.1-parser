using System;
using System.IO;

var path = args.Length > 0 ? args[0] : "input.luac"; // ud

byte[] file = File.ReadAllBytes(path);
Parser.parser(file);