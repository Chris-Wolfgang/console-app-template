# 0002 — Automatic command registration via an IConvention

- **Status:** Accepted
- **Date:** 2026-07-09

## Context

McMaster.Extensions.CommandLineUtils registers subcommands through
`[Subcommand(typeof(T))]` attributes on the root command. That means every new
command a user adds — including one scaffolded with `dotnet new cwsubcmd` —
required a manual edit to `Program.cs` to wire it up. That extra step was easy to
forget and was a long-standing papercut (the `cwsubcmd` gotcha).

## Decision

Ship `Framework/AutoRegisterCommandsConvention` — a McMaster `IConvention` that
scans the entry assembly for `[Command]`-decorated classes (excluding the root
`Program`) and registers each as a subcommand. `Program.cs` wires it via the
`RunCommandLineApplicationAsync` configure callback and drops the explicit
`[Subcommand(...)]` attribute (kept as a commented opt-out).

The subcommand name comes from `[Command].Name` when set, otherwise from the
class name with a trailing `Command` removed and lower-cased
(`SampleCommand` → `sample`) — identical to McMaster's previous behavior, so no
observable change for existing commands.

## Consequences

- New commands work with **zero `Program.cs` edits** — add a `[Command]` class
  (or run `dotnet new cwsubcmd`) and it's registered on the next run. The
  item-template comments say so.
- Explicitly-registered commands are skipped, so both styles coexist; a root-only
  guard prevents the convention recursing into the subcommands it registers.
- Registration is now reflection-at-startup rather than compile-time attributes.
  For a CLI app this cost is negligible; it is noted here because it is a
  deliberate trade (convenience over an attribute the reader can see).
- Users who prefer explicit wiring remove the `AddConvention` call and use
  `[Subcommand]` — documented in `Instructions.md`.
