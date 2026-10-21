type: fix

`--top-level --otel` now registers OpenTelemetry (`ConfigureApplicationTelemetry()`) in the top-level `Program.cs` exactly like the Main-based entry point, so spans and metrics are no longer silently dropped; the PR smoke matrix and the snapshot test now also generate, build and run an app with every opt-in option enabled.
