# 0003 — Item-template namespace auto-detection with a coalesce fallback

- **Status:** Accepted
- **Date:** 2026-07-09

## Context

The `cwsubcmd` / `cwsubcmdetl` item templates need to place the generated class
in the containing project's namespace (`<RootNamespace>.Command`). The natural
mechanism is a `bind` symbol to `msbuild:RootNamespace`. But that bind is only
evaluable once the project has been **restored** — MSBuild can't read the
property otherwise. Running `dotnet new cwsubcmd` on a freshly-created,
un-restored project failed the bind and emitted the literal `{DefaultNamespace}`
token — invalid C# that wouldn't compile. (The built-in `dotnet new class`
template sidesteps this by *requiring* restore via a project-capability
constraint.)

## Decision

Keep the `msbuild:RootNamespace` bind (it is correct when it resolves, in both
Visual Studio and a restored CLI project) but wrap it in a `coalesce` generated
symbol: `RootNamespaceBind` → `DefaultNamespaceFallback` parameter (default
`MyApp`, overridable with `--DefaultNamespaceFallback`). Restored → the real
namespace; un-restored → a valid placeholder instead of a broken token.

## Consequences

- Visual Studio and restored-CLI usage get the correct namespace automatically.
- Un-restored CLI usage produces compilable code (a placeholder namespace the
  user can fix, or override up-front) rather than a syntax error.
- `Instructions.md` documents "restore first for auto-detection, or pass
  `--DefaultNamespaceFallback`."
- The generated subcommand's compilation is guarded by the smoke matrix (ADR
  0004), which restores the app and builds the added subcommands — this is where
  the original bug, and later a latent `RCS1163`, were caught.
