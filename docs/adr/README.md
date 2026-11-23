# Architecture Decision Records

This directory records the significant, non-obvious decisions behind this
template pack — the "why" that isn't visible in the code itself. Each ADR is a
short, immutable record: once a decision is made it stays here; if it changes
later, a **new** ADR supersedes the old one (which is marked *Superseded* rather
than deleted, so the history stays intact).

Format: [Michael Nygard's ADR style](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions.html)
— Status, Context, Decision, Consequences.

## Index

See [index.md](index.md) for every record and its status, starting with
[ADR 0000](0000-record-architecture-decisions.md), which describes the practice.

## Adding an ADR

Copy [`TEMPLATE.md`](TEMPLATE.md), name it `NNNN-short-title.md` with the next
number, fill in Status/Context/Decision/Consequences, and add a row to
[`index.md`](index.md). Keep it short — an ADR is a paragraph of context and a paragraph of
decision, not a design doc.
