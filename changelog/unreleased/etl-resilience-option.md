type: feature

`dotnet new cwsubcmdetl --resilience` wraps the generated ETL command's extract/transform/load pass in a Polly pipeline (retry with exponential backoff, circuit breaker, per-attempt timeout) configured from a `Resilience` section in `AppSettings.json`, and adds `Polly.Core` to the project; the default output is unchanged.
