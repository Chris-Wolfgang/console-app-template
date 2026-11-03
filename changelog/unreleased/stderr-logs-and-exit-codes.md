type: fix

Generated apps keep stdout for command output: Serilog and the AOT console logger now write to stderr, command templates print only the error message to stderr instead of the full exception to stdout, cancellation returns `ExitCode.Canceled` from every command and entry point, and a missing `DOTNET_ENVIRONMENT` surfaces as an `InvalidOperationException` instead of `Environment.FailFast`.
