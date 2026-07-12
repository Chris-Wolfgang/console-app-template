# Threat Model (STRIDE)

A lightweight [STRIDE](https://learn.microsoft.com/azure/security/develop/threat-modeling-tool-threats)
threat model for this repository. The point is to name the realistic threats and
show which control already covers each — and where a consumer inherits
responsibility.

## Scope & assets

This is a `dotnet new` **template pack**, not a running service or a shipped
library. Its security-relevant assets are:

1. **The published template packages** — `Wolfgang.Template.Console*` on NuGet.
   Consumers install them and generate code that ends up in *their* products, so a
   compromise here propagates downstream.
2. **The publish / CI pipeline** — GitHub Actions workflows and the credentials
   that push to NuGet and `gh-pages`.
3. **The generated project's security defaults** — the posture a consumer inherits
   the moment they run `dotnet new`.

Out of scope: the runtime behaviour of the app a consumer *builds* on top of the
template — that's their threat model (see the "consumer inherits" notes below).

## STRIDE

### 1. Published template packages

| Threat | Example | Mitigation |
|--------|---------|------------|
| **S**poofing | A look-alike package or a forged "official" build | Fixed package IDs under `Wolfgang.Template.*`; publish only from the tagged release pipeline; **SLSA build-provenance attestation** lets consumers verify origin (`gh attestation verify`). |
| **T**ampering | A malicious dependency pulled into the generated app; altered package contents | **License audit** + **CycloneDX SBOM** enumerate the dependency set; **Dependabot** + SHA-pinned actions; attestation covers the package bytes. |
| **R**epudiation | "We didn't publish that version" | Releases are tag-driven and logged; provenance attestation ties each package to a workflow run + commit. |
| **I**nfo disclosure | A secret committed into a template payload and shipped | **gitleaks** secret scan on every PR + secret scanning/push protection on the repo. |
| **D**oS | n/a | Not a served asset; NuGet hosts the package. |
| **E**levation | A compromised publish key ships a rogue version | Least-privilege NuGet key (scoped, push-only, expiring); **OIDC trusted publishing** is the recommended upgrade (no long-lived key); see [DISASTER-RECOVERY.md](DISASTER-RECOVERY.md). |

### 2. Publish / CI pipeline

| Threat | Example | Mitigation |
|--------|---------|------------|
| **S**poofing | A PR from a fork runs with write scope | PRs use `pull_request` with `contents: read` and no secret exposure to PR code (see [ADR 0005](adr/0005-pr-trigger-pull-request.md)). |
| **T**ampering | A moved action tag pulls in malicious code; workflow-YAML injection | **All actions SHA-pinned**; **zizmor** + **actionlint** gate the workflows (template injection, unpinned uses, dangerous triggers); template tag values passed via `env:` not inline `${{ }}`. |
| **R**epudiation | Unattributed changes to workflows/config | Branch ruleset on `main` (required checks, review, non-fast-forward, deletion protection); audit log. |
| **I**nfo disclosure | `NUGET_API_KEY` leaked into logs/artifacts | `persist-credentials: false` on checkouts; secrets referenced only where needed; OIDC removes the long-lived key entirely. |
| **D**oS | n/a | — |
| **E**levation | An over-scoped `GITHUB_TOKEN` | Workflow-level `contents: read`; jobs opt into the minimum extra scope (e.g. `attestations: write`, `security-events: write`) only where required. |

### 3. Generated project's defaults (consumer inherits)

| Threat | Example | Mitigation / consumer responsibility |
|--------|---------|--------------------------------------|
| **I**nfo disclosure | Secrets logged, or written to log files | Serilog is configured with structured logging and a rolling file sink; **the consumer must not log secrets** and should review the sink's path/retention for their environment. |
| **T**ampering | Analyzer suppressions hiding real issues | Generated projects ship AsyncFixer/Meziantou/Roslynator/Sonar analyzers with warnings-as-errors in Release; the `<NoWarn>` set is scaffolding-scoped and documented. |
| **E**levation / misuse | The app runs with more privilege than it needs | The template is a neutral scaffold; **least-privilege at runtime is the consumer's responsibility** (file/network/credential access they add). |

## Maintaining this model

- **Review cadence:** revisit at least **annually** and on **every release**, and
  immediately when the attack surface changes — a new publish mechanism, a new
  template that reads untrusted input, or accepting fork PRs (which would flip the
  [ADR-0005](adr/0005-pr-trigger-pull-request.md) decision and re-introduce the
  fork-safety controls).
- **Act on findings:** any threat this review surfaces that isn't already covered
  becomes a tracked GitHub issue (labelled `security`), so it's followed up rather
  than living only in this document.
- Keep each row tied to a concrete control or an explicit "consumer owns this".
