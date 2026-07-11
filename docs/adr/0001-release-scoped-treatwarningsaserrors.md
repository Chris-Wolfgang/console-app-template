# 0001 — Release-scoped TreatWarningsAsErrors in generated projects

- **Status:** Accepted
- **Date:** 2026-07-09

## Context

The generated console app ships a curated analyzer set (AsyncFixer, Meziantou,
Roslynator, SonarAnalyzer) to hold users to a quality bar. For that bar to have
teeth, warnings must fail the build — but doing so in *Debug* would make everyday
development miserable (every in-progress TODO breaks the build).

A generated project also ships **no `.editorconfig`**, so it inherits whatever
analyzer severities the building SDK enables by default. Those defaults **drift
between SDK versions**, so "warnings as errors" turns any such drift into a build
break for the user — on an SDK the template author never tested.

## Decision

Enable `TreatWarningsAsErrors` **only for Release** builds
(`Condition="'$(Configuration)' == 'Release'"`), and pin the known
false-positives / intentional-scaffolding warnings in a single `<NoWarn>` in the
generated csproj: `MA0026;S1135;S1144;S125;MA0004;CA2007;CS9057`. Each entry has
a documented reason (guided-setup TODOs, unreferenced starter helpers, example
code in comments, ConfigureAwait-in-a-console-app, the McMaster source-generator
compiler-version mismatch on older SDKs).

## Consequences

- Debug builds stay warning-tolerant; Release enforces the bar. CI builds Release,
  so the gate is real without blocking local iteration.
- The `NoWarn` list is a maintenance surface: new SDKs can enable new default
  rules that break generated apps. The **generated-project smoke matrix** (ADR
  0004) is the safety net that catches these across OS × SDK before users hit them
  — it has already caught CS9057 (.NET 8 SDK), CA2007 (newer SDKs), and RCS1163.
- Users are told, in a csproj comment, to remove the scaffolding entries as they
  work through the TODOs.
