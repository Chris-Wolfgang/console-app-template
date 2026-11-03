# 0004 — Template-pack CI/release adaptations

- **Status:** Accepted
- **Date:** 2026-07-09

## Context

This repository started from the fleet's canonical `pr.yaml` / `release.yaml`,
which were written for **library/application** repos: they assume a root solution,
a test suite with a coverage gate, and a DocFX site. This repo is a `dotnet new`
**template pack** — the solution lives at `src/ConsoleAppTemplate.sln`, there are
no test projects, and `docfx_project/` is a placeholder. Running the canonical
workflows unchanged produced hard failures (MSB1003 from bare `dotnet restore` at
root; "release requires tests"; a docs-deploy crash on a missing `docfx.json`).

This ADR covers the **adaptations** that keep the build/release pipeline working
for a template pack while staying close to canonical. (`pr.yaml` later diverged
further, in a way that is *not* just an adaptation — see [ADR 0005](0005-pr-trigger-pull-request.md).)

## Decision

Adapt the canonical build/release steps to the template-pack shape rather than
fork them:

1. **Solution path** — every `dotnet restore/build/test` targets
   `src/ConsoleAppTemplate.sln` explicitly (no root solution exists).
2. **No-tests tolerance** — the release test + coverage-gate steps *skip with a
   notice* when no `*Test*.csproj` exists, instead of hard-failing. The hard
   failure is preserved for repos that do have tests.
3. **Conditional docs** — `validate-release` emits a `has-docfx` output and the
   docs-deploy job is gated on it, so a placeholder `docfx_project/` doesn't crash
   the release.
4. **Generated-project smoke matrix** — because the template's real "product" is
   the code it *generates* (which the repo itself never compiles as an app), a
   matrix job packs the templates, instantiates a project, and builds+runs it
   across OS × SDK. This is the only place the generated app is exercised the way
   a user would, in the environment a user gets (no `.editorconfig` → analyzer
   defaults; the user's SDK).

## Consequences

- `release.yaml` and the shared build steps stay close to canonical (still
  re-syncable from `repo-template`) while working for a repo with no root solution,
  no tests, and no docs site.
- The smoke matrix is load-bearing: it has caught multiple ship-broken-to-users
  bugs invisible to local dev (a C# 13-only escape breaking the .NET 8 SDK,
  CS9057, CA2007, RCS1163), because local dev used a newer SDK than the documented
  minimum. See ADR 0001.
- Coverage/perf/mutation-style CI that presumes a test suite or benchmarkable code
  is **not applicable** to this repo and belongs in the code-bearing repos.
- These are *adaptations*, not a fork — the build/release workflows remain
  recognizably canonical. The one place that is a genuine fork is `pr.yaml`'s
  trigger and fork-safety model, split out into [ADR 0005](0005-pr-trigger-pull-request.md).

## Addendum (2026-10-21) — inert fleet scaffolding is kept, not deleted

`repo-template` syncs are additive: a file deleted here comes back on the next sync and
the deletion has to be fought every time. So the fleet files that presuppose a
library/application repo stay in the tree, each with a short "Not applicable in this
template-pack repo" header pointing back here, and nothing else is built on them.
Issue #509 is the audit that produced this list.

| File(s) | Why it is inert here | Treatment |
|---------|----------------------|-----------|
| `.globalconfig`, `coverlet.runsettings`, `tests/.editorconfig` | No test project in `src/ConsoleAppTemplate.sln`; `.globalconfig` sits outside every `Content/` folder, so it never reaches a generated app | Header comment |
| `benchmarks/.gitkeep`, `.github/workflows/benchmarks.yaml` | `benchmarks/` is empty; the benchmarks the templates *generate* live under `src/*/Content/benchmarks` and belong to the generated app | Header comment; detection made generic (first tracked `benchmarks/**/*.csproj`, no placeholder project name) and the "not present" message demoted from `::notice::` to a plain log line so pushes to `src/**` stop annotating |
| `docfx_project/.gitkeep`, `.github/workflows/docfx.yaml`, `docs/DOCFX-VERSION-PICKER.md`, `scripts/Validate-DocsDeploy.ps1` | No DocFX site; `release.yaml`'s `detect-docfx` step already gates the docs job on `docfx_project/docfx.json` | Header comment / note |
| `.github/workflows/stryker.yaml`, `scripts/tfm-parity.ps1`, `scripts/build-pr.ps1` (test + coverage steps) | All no-ops without `tests/*.csproj`; `build-pr.ps1` already says so in its help | Header comment (`build-pr.ps1` unchanged) |
| `.github/workflows/sourcelink.yaml` | Nothing under `src/` targets `net10.0` (packers are `netstandard2.0` and ship no assembly; Content apps are `net8.0` payload), so its build loop skips every project and verifies no PDB | Header comment; **not** filtered to `*.template.pack.csproj` — there is nothing to verify either way |
| `.github/workflows/sbom.yaml`, `release.yaml` "Generate SBOM" step | Iterate every `src/**/*.csproj`, including the two Content app csprojs | **Kept on purpose.** The `*.template.pack.csproj` packers have no package dependencies, so an SBOM of them is empty; the generated app's dependency set is the SBOM `docs/THREAT-MODEL.md` relies on ("Tampering: a malicious dependency pulled into the generated app"). Comment added at both sites |
| `BannedSymbols.txt` (root) | Was a reference list no project consumed | **Wired.** Each content project (`ConsoleAppTemplate.csproj`, `AotConsoleTemplate.csproj`) now carries its own `Content/BannedSymbols.txt` as an `AdditionalFiles` item for `Microsoft.CodeAnalysis.BannedApiAnalyzers`, so a generated app fails its Release build (RS0030) on the listed blocking/obsolete APIs. The root file remains the fleet-synced reference copy; keep the two Content copies in step with it (they differ only in the header lines). Pinned to 3.3.4, the last build the .NET 8 SDK's compiler loads |

Also noted, not changed: `.editorconfig`'s `[src/**/*.cs]` block says `Directory.Build.props`
turns on `GenerateDocumentationFile` for `src/`; here each Content csproj sets it itself and
`Directory.Build.props` does not. The comment is synced from `repo-template`, so the fix
belongs there.
