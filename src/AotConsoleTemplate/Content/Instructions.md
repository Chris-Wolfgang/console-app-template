# Getting started with your Native AOT console app

This project was generated from the Wolfgang **cwconsole-aot** template. It is a console
application designed to publish as a [Native AOT](https://learn.microsoft.com/dotnet/core/deploying/native-aot/)
executable — small, fast-starting, self-contained, and JIT-free.

## First steps

1. **Run it:**
   ```sh
   dotnet run -- --name YourName
   ```
   You should see `Hello, YourName!`.

2. **Publish a native binary:**
   ```sh
   dotnet publish -c Release -r linux-x64
   ```
   Native AOT needs a platform C toolchain — see [the prerequisites](https://aka.ms/nativeaot-prerequisites).

## What's wired up

- **CLI surface — `Program.cs`.** Built with [System.CommandLine](https://learn.microsoft.com/dotnet/standard/commandline/).
  Add options, arguments and subcommands to the `RootCommand`. The parser is reflection-free,
  so it stays AOT-safe as you grow it.
- **Dependency injection + logging.** `Host.CreateApplicationBuilder` gives you the generic
  host. Register services on `builder.Services`; resolve them from `host.Services` inside a
  command action. `ILogger<T>` is available out of the box.
- **Configuration — `appsettings.json`.** Bound to `Model/SampleOptions` with the
  **source-generated** binder. Add sections and bind them the same way; do **not** switch to
  reflection-based `IConfiguration.Get<T>()` without the generator or you will break AOT.

## Keeping it AOT-safe

`PublishAot` is set in the csproj, so the AOT/trim analyzers run on every build. If you add a
package or API that needs reflection or runtime code generation, you will get `IL2026`/`IL3050`
warnings (errors under Release). Prefer source generators and explicit registration over
reflection. Serilog's configuration-driven setup, for example, is **not** AOT-safe — this is
why the template uses `Microsoft.Extensions.Logging` instead.

## TODO checklist

- [ ] Replace the sample `--name` option with your real CLI surface.
- [ ] Replace `IGreeter` / `SampleOptions` with your own services and configuration.
- [ ] Set `<RuntimeIdentifier>` (or pass `-r`) for the platforms you publish to.
- [ ] Update the copyright/author/description in the csproj as needed.
