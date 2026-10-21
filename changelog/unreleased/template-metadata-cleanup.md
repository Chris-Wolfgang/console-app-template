type: fix

Generated console apps no longer carry this repository's package metadata (`RepositoryUrl`, `PackageProjectUrl`, `Title`, `PackageIcon`, `PackageTags`) or a `Compile Remove` for `Program.TopLevel.cs` in the `--top-level` variant; the `sample` launch profile now names `AppSettings.json`; the Visual Studio template icons resolve; and the ETL sub-command pack no longer declares NuGet dependencies.
