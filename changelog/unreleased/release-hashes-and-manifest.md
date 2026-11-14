type: feature

Releases now publish `SHA256SUMS.txt` (package and per-entry hashes) and `BUILD-MANIFEST.txt` (SDK, runner image, commit), both covered by the build-provenance attestation; `docs/REPRODUCIBLE-BUILD.md` shows how to rebuild a tag and compare. This is the first release a consumer can verify that way.
