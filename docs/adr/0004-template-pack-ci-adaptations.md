# 0004 — Template-pack CI/release adaptations

- **Status:** Accepted
- **Date:** 2026-07-09 (amended 2026-07-11: added decision 5, the PR trigger)

## Context

This repository uses the fleet's canonical `pr.yaml` / `release.yaml`, which were
written for **library/application** repos: they assume a root solution, a test
suite with a coverage gate, and a DocFX site. This repo is a `dotnet new`
**template pack** — the solution lives at `src/ConsoleAppTemplate.sln`, there are
no test projects, and `docfx_project/` is a placeholder. Running the canonical
workflows unchanged produced hard failures (MSB1003 from bare `dotnet restore` at
root; "release requires tests"; a docs-deploy crash on a missing `docfx.json`).

## Decision

Adapt the canonical workflows to the template-pack shape rather than fork them:

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
5. **PR trigger — `pull_request`, not `pull_request_target`.** The canonical
   `pr.yaml` uses `pull_request_target` and a fork-safety apparatus (explicit
   `refs/pull/*/head` checkouts, fetching config files from `main`, and a
   "protected configuration file" guard) whose entire purpose is to run untrusted
   **fork** PRs safely. This repo's PRs come from same-repo branches (single
   maintainer), so that model is complexity with no benefit — and it costs real
   friction (a `dangerous-triggers` suppression, protected-file-PR splits, and CI
   validating `main`'s config instead of the PR's). We switched to plain
   `pull_request` and removed the fork-only apparatus, keeping every job and its
   name (so the branch ruleset's required checks still match).

## Consequences

- The workflows stay recognizably canonical (easy to re-sync from `repo-template`)
  while working for a repo with no root solution, no tests, and no docs site.
- The smoke matrix is load-bearing: it has caught multiple ship-broken-to-users
  bugs invisible to local dev (a C# 13-only escape breaking the .NET 8 SDK,
  CS9057, CA2007, RCS1163), because local dev used a newer SDK than the documented
  minimum. See ADR 0001.
- Coverage/perf/mutation-style CI that presumes a test suite or benchmarkable code
  is **not applicable** to this repo and belongs in the code-bearing repos.
- Decision 5 is a deliberate *divergence* from canonical (not just an adaptation):
  the `pull_request` trigger is the right default for a single-maintainer repo, but
  a repo that accepts fork PRs should keep `pull_request_target` and the fork-safety
  apparatus. Re-syncing `pr.yaml` from `repo-template` must preserve this choice.
