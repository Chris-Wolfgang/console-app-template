#!/usr/bin/env bash
#
# Snapshot (approval) test for the generated project output (issue #208).
#
# Packs + installs the cwconsole template, generates a project with fixed inputs,
# and produces a normalized manifest (one `sha256␠relative/path` line per file,
# sorted). Compares it to the committed golden so any *content or structure* drift
# in what the template generates fails CI — even changes that still compile, which
# the build-based smoke matrix can't see.
#
#   ./snapshot.sh --check    (default) regenerate and diff against the golden; nonzero on drift
#   ./snapshot.sh --update             regenerate and overwrite the golden (run after an intended change)
#
# Runs on Linux/macOS and Windows git-bash (needs: dotnet, sed, find, and
# sha256sum or shasum).
set -euo pipefail

MODE="${1:---check}"
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
GOLDEN="$REPO_ROOT/tests/snapshots/cwconsole.manifest"
PACK_PROJECT="$REPO_ROOT/src/ConsoleAppTemplate/dotnet.consoleapp.template.pack.csproj"

# Fixed name so the output is deterministic (name → namespaces). The author is
# left at its static default; the only dynamic value is the copyright year, which
# is normalized below.
APP_NAME="SnapshotApp"

work="$(mktemp -d)"
cleanup() { rm -rf "$work"; }
trap cleanup EXIT

# Portable SHA-256: Linux has `sha256sum`; macOS/BSD has `shasum -a 256`.
sha256() { if command -v sha256sum > /dev/null 2>&1; then sha256sum; else shasum -a 256; fi; }

echo "Packing cwconsole..."
dotnet pack "$PACK_PROJECT" -c Release -o "$work/pkg" > /dev/null

# Isolate the dotnet CLI home so `dotnet new install` uses a throwaway template
# hive under $work — it never touches (or overwrites) the contributor's global
# `dotnet new` templates, and nothing needs uninstalling afterwards.
export DOTNET_CLI_HOME="$work/cli-home"
mkdir -p "$DOTNET_CLI_HOME"

echo "Installing + generating in an isolated template hive..."
dotnet new install "$work"/pkg/*.nupkg > /dev/null

echo "Generating project..."
mkdir -p "$work/gen"
# Ignore a nonzero exit here (the template's "open Instructions.md" post-action
# is unsupported headless and returns 127); the manifest comparison is the real
# check and will catch a genuine generation failure as a diff.
( cd "$work/gen" && dotnet new cwconsole -n "$APP_NAME" > /dev/null 2>&1 ) || true

# Build the manifest, one `sha256␠relative/path` line per file.
# - `logs/` is excluded: those are gitignored runtime artifacts, not committed
#   template content, so a clean checkout (and published package) never ships them.
# - Text files: normalize the injected copyright year (a `now` generator) so the
#   snapshot doesn't churn every January.
# - Binary files (the icons): hash the raw bytes — running them through `sed` is
#   unsafe and gives platform-dependent results.
manifest="$work/manifest.txt"
: > "$manifest"
while IFS= read -r -d '' f; do
  rel="${f#"$work/gen/"}"
  if grep -Iq . "$f"; then
    hash="$(sed -E 's/(Copyright )[0-9]{4}/\1YYYY/g' "$f" | sha256 | cut -d' ' -f1)"
  else
    hash="$(sha256 < "$f" | cut -d' ' -f1)"
  fi
  printf '%s  %s\n' "$hash" "$rel" >> "$manifest"
done < <(find "$work/gen" -type f -not -path '*/logs/*' -print0)
LC_ALL=C sort -o "$manifest" "$manifest"

if [ "$MODE" = "--update" ]; then
  cp "$manifest" "$GOLDEN"
  echo "Updated golden: $GOLDEN ($(wc -l < "$GOLDEN") files)"
  exit 0
fi

if [ ! -f "$GOLDEN" ]; then
  echo "::error::No golden manifest at $GOLDEN. Run './snapshot.sh --update' to create it." >&2
  exit 1
fi

if diff -u "$GOLDEN" "$manifest"; then
  echo "Snapshot OK: generated output matches the golden ($(wc -l < "$GOLDEN") files)."
else
  echo "::error::Generated output drifted from the golden snapshot (diff above). If intended, run './tests/snapshots/snapshot.sh --update' and commit the change." >&2
  exit 1
fi
