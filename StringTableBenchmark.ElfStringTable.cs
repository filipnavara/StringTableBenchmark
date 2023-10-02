using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using LibObjectFile.Elf;

public partial class StringTableBenchmarks
{
    public ulong ElfStringTableByteSize;

    [Benchmark]
    public void ElfStringTable()
    {
        ElfStringTable elfStringTable = new();
        foreach (var str in inputData)
        {
            elfStringTable.GetOrCreateIndex(str);
        }
        ElfStringTableByteSize = elfStringTable.Size;
    }
}