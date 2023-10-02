using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

[MemoryDiagnoser]
//[EventPipeProfiler(EventPipeProfile.CpuSampling)]
public partial class StringTableBenchmarks
{
    private readonly string[] inputData;

    public StringTableBenchmarks()
    {
        inputData = File.ReadAllLines("input.txt");
    }
}