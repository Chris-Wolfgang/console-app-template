#!/usr/bin/env bash
#
# Snapshot (approval) test for the generated project output (issue #208).
#
# Packs + installs the cwconsole template, generates a project per option set with
# fixed inputs, and produces a normalized manifest (one `sha256␠relative/path` line
# per file, sorted) for each. Compares them to the committed goldens so any *content
# or structure* drift in what the template generates fails CI — even changes that
# still compile, which the build-based smoke matrix can't see.
#
# Two option sets are covered (issue #500): the defaults (cwconsole.manifest) and
# every opt-in switch enabled (cwconsole-all-options.manifest), so option-conditional
# content such as Program.TopLevel.cs and the OpenTelemetry wiring is snapshotted too.
#
#   ./snapshot.sh --check    (default) regenerate and diff against the goldens; nonzero on drift
#   ./snapshot.sh --update             regenerate and overwrite the goldens (run after an intended change)
#
# Runs on Linux/macOS and Windows git-bash (needs: dotnet, sed, find, and
# sha256sum or shasum).
set -euo pipefail

MODE="${1:---check}"
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
GOLDEN_DIR="$REPO_ROOT/tests/snapshots"
# Every opt-in template switch, so option-conditional content is snapshotted too.
ALL_OPTIONS="--otel --top-level --health-check --cli-contract --unit-tests --integration-tests --benchmarks"
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

# generate <variant> <golden-name> [template switches...]
# Generates one project under $work/gen/<variant> and writes $work/<variant>.manifest.
generate() {
  local variant="$1" golden="$2"; shift 2
  local gen="$work/gen/$variant"
  echo "Generating project ($variant: dotnet new cwconsole -n $APP_NAME $*)..."
  mkdir -p "$gen"
  # Ignore a nonzero exit here (the template's "open Instructions.md" post-action
  # is unsupported headless and returns 127); the manifest comparison is the real
  # check and will catch a genuine generation failure as a diff.
  ( cd "$gen" && dotnet new cwconsole -n "$APP_NAME" "$@" > /dev/null 2>&1 ) || true

  # Build the manifest, one `sha256␠relative/path` line per file.
  # - `logs/` is excluded: those are gitignored runtime artifacts, not committed
  #   template content, so a clean checkout (and published package) never ships them.
  # - Text files: normalize the injected copyright year (a `now` generator) so the
  #   snapshot doesn't churn every January.
  # - Binary files (the icons): hash the raw bytes — running them through `sed` is
  #   unsafe and gives platform-dependent results.
  local manifest="$work/$variant.manifest"
  : > "$manifest"
  while IFS= read -r -d '' f; do
    rel="${f#"$gen/"}"
    if grep -Iq . "$f"; then
      hash="$(sed -E 's/(Copyright )[0-9]{4}/\1YYYY/g' "$f" | sha256 | cut -d' ' -f1)"
    else
      hash="$(sha256 < "$f" | cut -d' ' -f1)"
    fi
    printf '%s  %s
' "$hash" "$rel" >> "$manifest"
  done < <(find "$gen" -type f -not -path '*/logs/*' -print0)
  LC_ALL=C sort -o "$manifest" "$manifest"
}

# compare <variant> <golden-name>: --update overwrites the golden; --check diffs against it.
compare() {
  local variant="$1" golden="$GOLDEN_DIR/$2" manifest="$work/$1.manifest"

  if [ "$MODE" = "--update" ]; then
    cp "$manifest" "$golden"
    echo "Updated golden: $golden ($(wc -l < "$golden") files)"
    return 0
  fi

  if [ ! -f "$golden" ]; then
    echo "::error::No golden manifest at $golden. Run './snapshot.sh --update' to create it." >&2
    return 1
  fi

  if diff -u "$golden" "$manifest"; then
    echo "Snapshot OK ($variant): generated output matches the golden ($(wc -l < "$golden") files)."
  else
    echo "::error::Generated output ($variant) drifted from the golden snapshot (diff above). If intended, run './tests/snapshots/snapshot.sh --update' and commit the change." >&2
    return 1
  fi
}

# shellcheck disable=SC2086  # ALL_OPTIONS is a deliberately word-split switch list
generate default     cwconsole.manifest
generate all-options cwconsole-all-options.manifest $ALL_OPTIONS

status=0
compare default     cwconsole.manifest             || status=1
compare all-options cwconsole-all-options.manifest || status=1
exit $status
