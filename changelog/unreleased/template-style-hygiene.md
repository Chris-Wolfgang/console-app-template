type: fix

Generated projects no longer carry BOMs, trailing whitespace, missing final newlines, block-scoped namespaces or mixed csproj indentation that `dotnet format` would churn on first run; the sample unit tests now exercise `AutoRegisterCommandsConvention.DeriveName` instead of re-asserting exit-code constants.
