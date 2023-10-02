using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;

public partial class StringTableBenchmarks
{
    public ulong NaiveByteSize;

    [Benchmark]
    public void Naive()
    {
        // Output data
        var stringTable = new ArrayBufferWriter<byte>();
        var stringToOffset = new Dictionary<string, int>(inputData.Length);

        for (int i = 0; i < inputData.Length; i++)
        {
            var str = inputData[i];
            stringToOffset.Add(str, stringTable.WrittenCount);
            int utf8Length = Encoding.UTF8.GetByteCount(str.AsSpan());
            var utf8Span = stringTable.GetSpan(utf8Length + 1);
            Encoding.UTF8.GetBytes(str, utf8Span);
            utf8Span[utf8Length] = 0;
            stringTable.Advance(utf8Length + 1);
        }

        NaiveByteSize = (ulong)stringTable.WrittenCount;
    }
}