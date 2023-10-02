using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public partial class StringTableBenchmarks
{
    private readonly string[] inputData;

    public StringTableBenchmarks()
    {
        inputData = File.ReadAllLines("input.txt");
    }
}