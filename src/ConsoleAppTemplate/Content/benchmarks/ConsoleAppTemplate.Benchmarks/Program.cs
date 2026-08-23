using BenchmarkDotNet.Running;

// Runs every [Benchmark] in this assembly. Invoke in Release for meaningful numbers:
//   dotnet run -c Release --project benchmarks/ConsoleAppTemplate.Benchmarks
// Filter to one class/method with args, e.g. `-- --filter *SampleBenchmarks*`.
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
