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
