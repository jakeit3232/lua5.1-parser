hi, this is a lua5.1 parser i've made.. usage is


```
byte[] file = File.ReadAllBytes(path);
Parser.parser(file);```

or if u wanna use the cli u can do 

```
dotnet run input.luac 
```
orrr u can adjust the file path in Program.cs

``
var path = args.Length > 0 ? args[0] : "input.luac"; -- change the luac file to whatever u like```
