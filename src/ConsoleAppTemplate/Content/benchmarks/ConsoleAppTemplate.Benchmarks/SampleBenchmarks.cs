using BenchmarkDotNet.Attributes;

namespace ConsoleAppTemplate.Benchmarks;

/// <summary>
/// Sample benchmark. Replace the body with the hot path you care about — a parser, a
/// transform, a serialization step — and add more <see cref="BenchmarkAttribute"/>
/// methods (optionally with <c>[Params]</c> inputs) to compare implementations.
/// </summary>
[MemoryDiagnoser]
public class SampleBenchmarks
{
    [Params(100, 1000)]
    public int Size { get; set; }



    [Benchmark]
    public long SumRange()
    {
        long total = 0;
        for (var i = 0; i < Size; i++)
        {
            total += i;
        }

        return total;
    }
}
