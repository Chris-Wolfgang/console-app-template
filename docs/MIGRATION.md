# Migration Guide

How to update when a new version of the Wolfgang console templates makes a
**breaking change**. Because these are `dotnet new` templates, "breaking" means a
change to the **template's CLI surface** (a parameter name or its accepted values)
or to the **generated project's structure/conventions** — not a runtime API.

Versions follow [SemVer](https://semver.org/). Breaking changes are always listed
here and flagged **BREAKING** in the [CHANGELOG](../CHANGELOG.md); pre-1.0 they may
land in a MINOR release.

Already-generated projects are unaffected by template changes — migration only
matters if you **re-scaffold** (especially from a script or CI that pins parameter
values).

---

## Unreleased

### `--response-file` choice values shortened

Affects `cwconsole`, `cwsubcmd`, and `cwsubcmdetl`. The `--response-file`
parameter's accepted values dropped the internal `ParseArgsAs…` prefix. The
generated code is identical; only the value you pass on the command line changed.

| Before | After |
|--------|-------|
| `--response-file ParseArgsAsLineSeparated` | `--response-file LineSeparated` |
| `--response-file ParseArgsAsSpaceSeparated` | `--response-file SpaceSeparated` |
| `--response-file Disabled` | `--response-file Disabled` *(unchanged)* |

The default also changed from `ParseArgsAsLineSeparated` to `LineSeparated`.

**Action:** if you scaffold from a script/CI that passes `--response-file
ParseArgsAs…`, update the value. Interactive users who pick from the prompt are
unaffected.

---

## Adding an entry

When a change renames/retypes a template parameter, changes an accepted value, or
alters the generated project's shape, add a section here (newest first) with a
before/after table and a one-line **Action**, and mirror it in the CHANGELOG under
a **BREAKING** note.
