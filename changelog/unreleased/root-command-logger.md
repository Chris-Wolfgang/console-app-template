type: fix

Running a generated app with no arguments now prints help and exits 0 instead of exiting 10 with "Unsupported type on OnExecute": the root command handler takes `ILogger<Program>`, which the host registers.
