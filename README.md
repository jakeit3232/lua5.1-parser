hi, this is a lua5.1 bytecode parser i've made.. usage is

```csharp
byte[] file = File.ReadAllBytes(path);
Parser.parser(file);
```

or if u wanna use the cli u can do

```bash
dotnet run input.luac
```

```csharp
var path = args.Length > 0 ? args[0] : "input.luac"; // change the luac file to whatever u like
```
