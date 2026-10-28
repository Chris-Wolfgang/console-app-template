type: fix

Generated apps now set `<Version>0.1.0</Version>` instead of `<PackageVersion>`, so `--version` and the logged `Version` property report 0.1.0 rather than 1.0.0 (cwconsole and cwconsole-aot).
