type: fix

The `--cli-contract` workflow now pins `actions/checkout` and `actions/setup-dotnet` by SHA and installs the SDK matching the chosen `--framework` (e.g. `10.0.x` for `net10.0`); the check scripts build explicitly so compiler errors are shown, and `cli-surface --output` writes the baseline atomically.
