# Keeping your CLI contract stable

Your app's **CLI surface** — its commands, subcommands, options (long/short names and
value kinds) and positional arguments — is a public contract. Scripts, CI pipelines and
other tools invoke it with specific flags, so silently renaming `--output` to `--out`,
removing a subcommand, or reordering a required argument **breaks downstream callers**.

This folder gives you a way to *know what CLI arguments you published* and get warned when
a change breaks that contract between releases.

## How it works

- The generated `cli-surface` subcommand walks the command model and emits a deterministic
  JSON manifest of the whole CLI surface:

  ```sh
  dotnet run -- cli-surface --output cli-contract/cli-surface.baseline.json
  ```

  (Use `--output`; without it the manifest goes to stdout, mixed with any log lines.)

- `cli-surface.baseline.json` is the committed snapshot of that surface. Create it once
  with the `--update` switch below, then commit it.

- The check script re-captures the current surface and diffs it against the baseline,
  failing on any difference.

## Everyday use

Create or refresh the baseline (do this deliberately — a change here means your published
CLI changed; note breaking changes in your release notes):

```sh
# Windows / cross-platform
pwsh cli-contract/check-cli-contract.ps1 -Update

# Linux / macOS
bash cli-contract/check-cli-contract.sh --update
```

Verify no drift (what CI runs):

```sh
pwsh cli-contract/check-cli-contract.ps1     # or: bash cli-contract/check-cli-contract.sh
```

## CI

`.github/workflows/cli-contract.yml` runs the check on every pull request and on pushes to
`main`. A PR that changes the CLI surface fails the check until you refresh and commit the
baseline — making every CLI-contract change explicit and reviewable.

## Adding an option or command safely

- **Additions** (new option/subcommand) are usually safe for existing callers, but the
  check still flags them so the baseline stays current — refresh and commit.
- **Renames / removals** are breaking. Prefer adding the new form and keeping the old one
  (even if hidden) for a deprecation period, then remove it in a major release.
