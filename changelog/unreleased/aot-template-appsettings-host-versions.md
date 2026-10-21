type: fix

The `cwconsole-aot` template now copies `appsettings.json` to the output on case-sensitive filesystems (the csproj item was spelled `AppSettings.json`), starts and stops the generic host around the command so registered `IHostedService`s run, and ships the same analyzer and `Microsoft.Extensions.Hosting` versions as `cwconsole`.
