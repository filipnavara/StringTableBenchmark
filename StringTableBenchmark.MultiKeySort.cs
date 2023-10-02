using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;

public partial class StringTableBenchmarks
{
    [Benchmark]
    public void MultiKeySort()
    {
        // Copy the input
        Span<string> inputSpan = inputData.ToArray();

        // Output data
        var stringTable = new ArrayBufferWriter<byte>();
        var stringToOffset = new Dictionary<string, int>(inputSpan.Length);

        MultiKeySort(inputSpan, 0);
        string? lastSymbol = null;
        int lastSymbolOffset = 0;
        for (int i = 0; i < inputSpan.Length; i++)
        {
            var str = inputSpan[i];
            if (lastSymbol != null && lastSymbol.EndsWith(str))
            {
                // suffix compressed
                stringToOffset.Add(str, lastSymbolOffset + Encoding.UTF8.GetByteCount(lastSymbol) - Encoding.UTF8.GetByteCount(str));
            }
            else
            {
                lastSymbol = str;
                lastSymbolOffset = stringTable.WrittenCount;

                int utf8Length = Encoding.UTF8.GetByteCount(str.AsSpan());
                var utf8Span = stringTable.GetSpan(utf8Length + 1);
                Encoding.UTF8.GetBytes(str, utf8Span);
                utf8Span[utf8Length] = 0;
                stringTable.Advance(utf8Length + 1);

                stringToOffset.Add(str, lastSymbolOffset);
            }
        }

        static char TailCharacter(string str, int pos) => pos < str.Length ? str[str.Length - pos - 1] : (char)0;

        static void MultiKeySort(Span<string> input, int pos)
        {
            if (input.Length <= 1)
                return;

            char pivot = TailCharacter(input[0], pos);
            int l = 0, h = input.Length;
            for (int i = 0; i < h;)
            {
                char c =  TailCharacter(input[i], pos);
                if (c > pivot)
                {
                    (input[l], input[i]) = (input[i], input[l]);
                    l++; i++;
                }
                else if (c < pivot)
                {
                    h--;
                    (input[h], input[i]) = (input[i], input[h]);
                }
                else
                {
                    i++;
                }
            }

            MultiKeySort(input.Slice(0, l), pos);
            MultiKeySort(input.Slice(l, h - l), pos + 1);
            MultiKeySort(input.Slice(h), pos);
        }
    }
}