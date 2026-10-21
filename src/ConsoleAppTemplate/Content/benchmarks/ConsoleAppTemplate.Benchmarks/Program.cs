using BenchmarkDotNet.Running;
using ConsoleAppTemplate.Benchmarks;

// Runs every [Benchmark] in this assembly. Invoke in Release for meaningful numbers:
//   dotnet run -c Release --project benchmarks/ConsoleAppTemplate.Benchmarks
// Filter to one class/method with args, e.g. `-- --filter *SampleBenchmarks*`.
// Anchor on a benchmark class rather than Program: with --top-level the app's own
// Program lives in the global namespace and is visible here via InternalsVisibleTo,
// so typeof(Program) would be ambiguous (CS0436) between the two.
BenchmarkSwitcher.FromAssembly(typeof(SampleBenchmarks).Assembly).Run(args);
