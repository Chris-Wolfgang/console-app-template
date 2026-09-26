type: internal

The console template pack project suppresses NU5110/NU5111 for `check-cli-contract.ps1`, which is template content rather than a NuGet install script, so it packs cleanly with warnings as errors.
