type: breaking

`cwconsole` and `cwconsole-aot` now generate a `net10.0` project, and `--framework` accepts only `net10.0` (the `net8.0` and `net9.0` choices are removed): the Microsoft.Extensions and Serilog 11.0 packages the templates reference do not support net8.0 or net9.0. Generating a project now needs the .NET 10.0 SDK or later.
