# Verifying a release (reproducible build)

This repository publishes four `dotnet new` template packages:
`Wolfgang.Template.Console`, `Wolfgang.Template.Console.Subcommand`,
`Wolfgang.Template.Console.ETL-SubCommand` and `Wolfgang.Template.Console.Aot`.
This page shows how to check that a published package was built from the tagged source,
without trusting the maintainer.

## What a release publishes

Every GitHub Release carries, next to the `.nupkg` files:

| Asset | Contents |
|---|---|
| `SHA256SUMS.txt` | One line per `.nupkg` (`<sha256>  <file>`), then one line per file inside each package (`<sha256>  <package file>/<entry path>`). |
| `BUILD-MANIFEST.txt` | Tag, commit, the exact .NET SDK version (`dotnet --version`), runner OS and image, the `CI` / `ContinuousIntegrationBuild` / `Deterministic` values and the pack command. |
| `*.intoto.jsonl` | The SLSA build-provenance attestation bundle. |

The packages, `SHA256SUMS.txt` and `BUILD-MANIFEST.txt` are all subjects of that attestation.

Releases cut before these assets were added (v0.8.0 and earlier) have no `SHA256SUMS.txt` or
`BUILD-MANIFEST.txt`; only their packages are attested.

## What must match, and what may differ

- **Must match:** every package entry line in `SHA256SUMS.txt` (the template payload, the
  `.nuspec`, the README and icons). The packages contain template source only. They ship no
  compiled `.dll` or `.pdb` (`IncludeBuildOutput=false`), so there are no per-TFM assembly
  hashes.
- **Expected to differ:** the whole-`.nupkg` hash lines. NuGet writes a random
  `package/services/metadata/core-properties/*.psmdcp` part (and the `_rels/.rels` that names
  it) and zip timestamps into every package, so no rebuild reproduces the container byte for
  byte. Those two OPC metadata entries are left out of the entry lines for the same reason.
  The [`reproducible-build.yml`](../.github/workflows/reproducible-build.yml) check packs
  every template twice on each change under `src/` and fails if the payload ever differs.
- **Cross-OS:** the release packs on the OS recorded in `BUILD-MANIFEST.txt`. `.gitattributes`
  pins LF line endings, so another OS is expected to match, but only the recorded OS is
  what the bar promises.

## Steps

The commands below use `bash`, `unzip`, GNU `find` and `sha256sum` (Linux or Git Bash on
Windows; on macOS install GNU findutils and coreutils) and the GitHub CLI.

1. **Download the release assets** for the tag you want to check, for example `v0.9.0`:

   ```bash
   TAG=v0.9.0
   mkdir -p published && cd published
   gh release download "$TAG" --repo Chris-Wolfgang/console-app-template \
     --pattern '*.nupkg' --pattern 'SHA256SUMS.txt' --pattern 'BUILD-MANIFEST.txt'
   cd ..
   ```

2. **Verify the attestation** on each downloaded asset:

   ```bash
   for f in published/*.nupkg published/SHA256SUMS.txt published/BUILD-MANIFEST.txt; do
     gh attestation verify "$f" --repo Chris-Wolfgang/console-app-template
   done
   ```

3. **Check the downloaded packages** against the published hash list:

   ```bash
   (cd published && grep '\.nupkg$' SHA256SUMS.txt | sha256sum -c -)
   ```

4. **Rebuild from the tag** with the SDK named in `BUILD-MANIFEST.txt` (`dotnet-sdk:`). The
   repository's `global.json` pins the SDK major; install the exact version from the manifest
   so the build uses it.

   ```bash
   git clone --branch "$TAG" https://github.com/Chris-Wolfgang/console-app-template.git src-tag
   cd src-tag
   dotnet --version   # must print the manifest's dotnet-sdk value
   for proj in src/*/*.template.pack.csproj; do
     CI=true dotnet pack "$proj" --configuration Release --output ../rebuilt \
       "-p:PackageVersion=${TAG#v}"
   done
   cd ..
   ```

5. **Hash the rebuilt package entries** in the same format and compare:

   ```bash
   cd rebuilt
   for pkg in *.nupkg; do mkdir -p "x/$pkg" && unzip -oq "$pkg" -d "x/$pkg"; done
   (cd x && find . -type f ! -name '*.psmdcp' ! -path '*/_rels/.rels' -printf '%P\n' \
     | LC_ALL=C sort \
     | while IFS= read -r f; do printf '%s  %s\n' "$(sha256sum < "$f" | cut -d' ' -f1)" "$f"; done) \
     > ../rebuilt-entries.txt
   cd ..
   grep -v '\.nupkg$' published/SHA256SUMS.txt | LC_ALL=C sort -k2 > published-entries.txt
   LC_ALL=C sort -k2 rebuilt-entries.txt | diff published-entries.txt - && echo "reproduced"
   ```

   An empty diff means every file in the published packages was rebuilt byte for byte from
   the tagged source.

## If it does not match

- Check `dotnet --version` against the manifest first; a different SDK can change the
  generated `.nuspec`.
- Check that the clone did not rewrite line endings (`git config core.autocrlf` should not
  override `.gitattributes`; a fresh clone honours it).
- Anything else is a real finding: open an issue with the diff, or follow
  [SECURITY.md](../SECURITY.md) if you suspect the published package was tampered with.
