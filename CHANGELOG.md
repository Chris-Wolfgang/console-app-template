# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

### Deprecated

### Removed

### Fixed

### Security

## [0.6.1] - 2026-08-23

_This release ships **`Wolfgang.Template.Console`** only. `Wolfgang.Template.Subcommand` and `Wolfgang.Template.ETL-SubCommand` remain at 0.6.0 — no changes in this cycle._

### Changed

- `SampleConfiguration.CommandTimeout` uses `init` instead of `set` — idiomatic
  record-immutability semantics; `Microsoft.Extensions.Configuration` binding is unaffected (#310, #318).
- Removed an unused `using ConsoleAppTemplate.Command;` from `Program.cs` (#310, #318).
- Analyzer dependency bumps in the generated project's csproj: `Meziantou.Analyzer`
  3.0.122 → 3.0.152 (#312), `Microsoft.Extensions.Hosting` 10.0.10 → 10.0.11 (#313),
  `Roslynator.Analyzers` 4.15.0 → 4.16.0 (#314), `SonarAnalyzer.CSharp`
  10.29.0.143774 → 10.32.0.713 (#315).

### Fixed

- File-scope `// ReSharper disable NotAccessedField.Compiler` on `Framework/ConsoleColors.cs`
  mirrors the existing csproj `<NoWarn>S1144</NoWarn>` for Sonar — the starter-helper palette
  is intentional scaffolding for the user's generated app to consume, but was flooding
  InspectCode with 38 false positives (#310, #318).

### Security

- OSSF Scorecard workflow now filters known won't-fix findings at the SARIF layer
  before code-scanning upload (`BranchProtectionID`, `CodeReviewID`, `CIIBestPracticesID`,
  `FuzzingID` whole-rule; `PinnedDependenciesID` `nugetCommand`/`pipCommand` sub-checks).
  The published Scorecard score is unaffected — only the Code Scanning tab is trimmed so
  real regressions stay visible (#310, #319).

## [0.6.0] - 2026-07-13

### Added

- Optional **`health` subcommand** (opt-in via `--health-check`) that validates runtime
  dependencies/configuration, with a `--json` flag for container health probes (#119).
- **Architecture Decision Records** under `docs/adr/` (#218), and a **migration guide**
  (`docs/MIGRATION.md`) for breaking template changes (#217).
- **STRIDE threat model** (`docs/THREAT-MODEL.md`) tying each threat to its control (#210).
- **Dependency license audit** — fails the build if the generated app's runtime graph pulls a
  non-permissive license (#216).
- **OSSF Scorecard** workflow uploading results to Code Scanning (#220).
- **SLSA build-provenance attestation** on the published packages (#206).
- **Workflow security audit** — a zizmor + actionlint gate over all workflows (#221) — and a
  **ReSharper InspectCode** check in PR CI (#235).
- **Reproducible-build verification** for the template packages (#214) and a **snapshot test**
  of the generated project output (#208).

### Changed

- **BREAKING:** the `--response-file` template parameter's choice values were shortened —
  `ParseArgsAsLineSeparated` → `LineSeparated`, `ParseArgsAsSpaceSeparated` → `SpaceSeparated`
  (default is now `LineSeparated`). Scripts/CI that pin the old values must update. See the
  [Migration Guide](docs/MIGRATION.md).
- Publishing now uses **NuGet Trusted Publishing (OIDC)** instead of a long-lived
  `NUGET_API_KEY` secret (#283).
- `pr.yaml` now triggers on **`pull_request`** (was `pull_request_target`) and drops the
  fork-safety apparatus that only applies to untrusted fork PRs (#278, [ADR 0005](docs/adr/0005-pr-trigger-pull-request.md)).
- All GitHub Actions are **SHA-pinned** (#221).

### Security

- Supply-chain hardening across the board: SHA-pinned actions, CycloneDX SBOM + SLSA
  attestation, permissive-only license audit, OSSF Scorecard, a zizmor/actionlint workflow
  audit, and OIDC publishing (no long-lived key to leak or rotate).
