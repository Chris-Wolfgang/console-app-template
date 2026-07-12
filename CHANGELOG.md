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

## [0.6.0] - 2026-07-12

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
