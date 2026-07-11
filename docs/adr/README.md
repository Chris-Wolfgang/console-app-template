# Architecture Decision Records

This directory records the significant, non-obvious decisions behind this
template pack — the "why" that isn't visible in the code itself. Each ADR is a
short, immutable record: once a decision is made it stays here; if it changes
later, a **new** ADR supersedes the old one (which is marked *Superseded* rather
than deleted, so the history stays intact).

Format: [Michael Nygard's ADR style](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions.html)
— Status, Context, Decision, Consequences.

## Index

| # | Title | Status |
|---|-------|--------|
| [0001](0001-release-scoped-treatwarningsaserrors.md) | Release-scoped TreatWarningsAsErrors in generated projects | Accepted |
| [0002](0002-automatic-command-registration.md) | Automatic command registration via an IConvention | Accepted |
| [0003](0003-item-template-namespace-auto-detection.md) | Item-template namespace auto-detection with a coalesce fallback | Accepted |
| [0004](0004-template-pack-ci-adaptations.md) | Template-pack CI/release adaptations | Accepted |

## Adding an ADR

Copy the format of an existing record, take the next number, and add a row to
the index. Keep it short — an ADR is a paragraph of context and a paragraph of
decision, not a design doc.
