# 0005 — `pr.yaml` diverges from canonical: `pull_request`, no fork-safety apparatus

- **Status:** Accepted
- **Date:** 2026-07-11

## Context

The canonical `pr.yaml` (adapted for this repo in [ADR 0004](0004-template-pack-ci-adaptations.md))
triggers on **`pull_request_target`** and carries a fork-safety apparatus whose
entire purpose is to run untrusted **fork** PRs without leaking secrets:

- every checkout uses an explicit `ref: refs/pull/<n>/head`,
- a step fetches the protected config files (`.editorconfig`, `Directory.Build.props`,
  …) from `main` so a PR can't override them,
- a "Detect protected configuration file changes" guard **fails** the run if a PR
  touches those files.

This repo's PRs come from **same-repo branches (single maintainer)**, not forks.
So the fork model is complexity with no benefit — and it costs real, recurring
friction:

- `pull_request_target` is a dangerous trigger that the workflow-security audit
  (zizmor) flags, forcing an explicit `# zizmor: ignore[dangerous-triggers]`;
- the protected-file guard means any PR that legitimately edits a workflow or
  config file has to be split into a separate admin-bypass PR;
- CI validates **`main`'s** copy of the config, not the PR's — so a PR's own
  workflow/config changes are never actually exercised before merge.

## Decision

Switch `pr.yaml` to plain **`pull_request`** and remove the fork-only apparatus:

- drop the explicit `refs/pull/<n>/head` checkouts (the default ref under
  `pull_request` already checks out the PR),
- drop the "fetch trusted configuration files from `main`" steps,
- drop the "Detect protected configuration file changes" guard (its premise —
  "CI uses `main`'s version to prevent bypasses" — is *false* under `pull_request`,
  which tests the PR's own files).

Keep **every job and its name** so the branch ruleset's required status checks
still match, and SHA-pin the actions. This is a deliberate **divergence** from the
canonical workflow, not an adaptation.

## Consequences

- Simpler `pr.yaml`; PR checks run against the PR's own code and config (what you
  actually want to validate before merge).
- The dangerous trigger is gone, so zizmor passes with no suppression, and
  workflow/config PRs no longer need protected-file splits.
- **Re-syncing `pr.yaml` from `repo-template` must NOT reintroduce
  `pull_request_target` or the fork-safety apparatus** — that would silently
  revert this decision. A template re-sync should treat `pr.yaml` as diverged.
- **If this repo ever accepts fork PRs**, revisit this: the canonical
  `pull_request_target` + fork-safety model is the correct choice there, and this
  ADR would be superseded.
