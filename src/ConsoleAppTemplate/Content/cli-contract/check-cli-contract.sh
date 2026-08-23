#!/usr/bin/env bash
#
# Verifies the app's CLI surface has not drifted from the committed baseline.
#
# Runs the 'cli-surface' subcommand to capture the current CLI manifest and diffs it
# against cli-contract/cli-surface.baseline.json. Exits non-zero on any difference - a
# renamed/removed option or subcommand is a breaking change for scripts and pipelines
# that call this app. Pass --update to accept the current surface as the new baseline
# (do this deliberately, and note breaking changes in your release notes).
#
# Usage:
#   cli-contract/check-cli-contract.sh
#   cli-contract/check-cli-contract.sh --update
set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
root_dir="$(dirname "$script_dir")"
baseline="$script_dir/cli-surface.baseline.json"
current="$(mktemp)"
trap 'rm -f "$current"' EXIT

dotnet run --project "$root_dir" -c Release -- cli-surface --output "$current" >/dev/null

if [[ "${1:-}" == "--update" ]]; then
    cp "$current" "$baseline"
    echo "Baseline updated: $baseline"
    exit 0
fi

if [[ ! -f "$baseline" ]]; then
    echo "No baseline found. Create one with: cli-contract/check-cli-contract.sh --update" >&2
    exit 1
fi

if ! diff -u "$baseline" "$current"; then
    echo "" >&2
    echo "CLI contract drift detected. If this change is intentional, re-run with --update and document it." >&2
    exit 1
fi

echo "CLI surface matches the baseline."
