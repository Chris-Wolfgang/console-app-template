type: feature

Generated `cwconsole` and `cwconsole-aot` apps now ship a `BannedSymbols.txt` wired into `Microsoft.CodeAnalysis.BannedApiAnalyzers`, so blocking and obsolete APIs (`Task.Wait`, `.Result`, `Thread.Sleep`, sync `File.*`, `WebClient`, `BinaryFormatter`, `DateTime.Now`, ...) are RS0030 build errors in Release instead of review guidelines.
