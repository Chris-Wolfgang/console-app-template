# Workflow Security

How the GitHub Actions workflows in this repository — above all the PR validation
workflow, `.github/workflows/pr.yaml` — limit what a pull request can do. The
threat-by-threat analysis lives in the [Threat Model](THREAT-MODEL.md) (section
"Publish / CI pipeline"); this page describes the mechanisms.

## The model: `pull_request`, same-repo branches only

`pr.yaml` triggers on plain **`pull_request`** (plus `push` to `main`). PRs in this
repo come from same-repo branches by a single maintainer, not from forks, so the
workflow checks out and validates **the PR's own code and config** — including any
change the PR makes to `pr.yaml` itself or to `.editorconfig` /
`Directory.Build.props`. That is what you want validated before merge.

This is a deliberate divergence from the canonical `repo-template` workflow, which
uses `pull_request_target` with a fork-safety apparatus (checkouts of
`refs/pull/<n>/head`, config files re-fetched from `main`, a "protected
configuration file" guard). Those mechanisms only make sense for **untrusted fork**
PRs and were removed here in [ADR 0005](adr/0005-pr-trigger-pull-request.md). Do not
reintroduce them when re-syncing `pr.yaml` from the template; if this repo ever
accepts fork PRs, revisit that ADR first.

Under `pull_request` the `GITHUB_TOKEN` is read-only for the PR run, secrets are not
exposed to code from a fork, and a PR cannot grant itself more than the permissions
declared in the workflow file it ships.

## Controls in the workflows

### Least-privilege token

Workflows declare `permissions: contents: read` at the top (`scorecard.yaml`
declares its read scopes per job instead). Jobs that need more opt in individually, with the minimum extra scope and a comment saying why —
for example `inspectcode-upload` in `pr.yaml` adds `security-events: write` and
`actions: read` for `upload-sarif`, and the release workflow's publish job adds
`id-token: write` for NuGet Trusted Publishing (OIDC). No workflow uses a
long-lived publish secret.

### `persist-credentials: false`

Every `actions/checkout` step sets `persist-credentials: false`, so the checkout
token is never written into `.git/config` where a later step (or a script the PR
adds) could read it. It does not hide `GITHUB_TOKEN` from steps that are passed it
explicitly; the least-privilege permissions above bound what such a step can do.

### Pinned actions and a workflow audit

All third-party actions are pinned to a full commit SHA with the version in a
trailing comment (`uses: owner/action@<sha> # vX.Y.Z`; `scripts/pin-actions.ps1`
resolves them). `workflow-security.yml` and `actions-audit.yaml` run **zizmor** and
**actionlint** over every workflow and fail on dangerous triggers, unpinned actions,
template injection, over-broad permissions and syntax errors. Template
values (tag names, versions) are passed through `env:` rather than interpolated
inline with `${{ }}`.

### Scanners that run on every PR

`pr.yaml` runs gitleaks (secrets), DevSkim and ReSharper InspectCode (SARIF uploaded
to Code Scanning); `semgrep.yaml`, `scorecard.yaml` and the generated-output
`snapshot.yml` run alongside. The branch ruleset on `main` requires the PR checks
to pass, so a PR cannot be merged with a failing or missing gate.

## Reviewing a workflow change

A PR that edits anything under `.github/workflows/` is validated by its own copy of
the workflow, so review it for:

1. the trigger — still `pull_request` / `push` / `workflow_dispatch` / `schedule`,
   never `pull_request_target` or `workflow_run` on PR content;
2. `permissions:` — top-level `contents: read` kept, any job-level additions
   justified in a comment;
3. `persist-credentials: false` on every checkout;
4. every `uses:` pinned to a full SHA with a version comment;
5. a green `Workflow Security Audit (zizmor + actionlint)` run.

## References

- [ADR 0005 — `pr.yaml` diverges from canonical: `pull_request`, no fork-safety apparatus](adr/0005-pr-trigger-pull-request.md)
- [Threat Model (STRIDE)](THREAT-MODEL.md)
- [GitHub Actions Security Hardening](https://docs.github.com/en/actions/security-guides/security-hardening-for-github-actions)
- [zizmor](https://github.com/zizmorcore/zizmor) · [actionlint](https://github.com/rhysd/actionlint)
