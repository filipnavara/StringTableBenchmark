using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;

public partial class StringTableBenchmarks
{
    public ulong MultiKeySortByteSize;

    [Benchmark]
    public void MultiKeySort()
    {
        // Copy the input
        Span<string> inputSpan = inputData.ToArray();

        // Output data
        var stringTable = new ArrayBufferWriter<byte>();
        var stringToOffset = new Dictionary<string, int>(inputSpan.Length);

        // Pre-sort the string based on their matching suffix
        MultiKeySort(inputSpan, 0);

        // Compose the final string table
        string? lastSymbol = null;
        for (int i = 0; i < inputSpan.Length; i++)
        {
            var str = inputSpan[i];
            if (lastSymbol != null && lastSymbol.EndsWith(str, StringComparison.Ordinal))
            {
                // Suffix matches the last symbol
                stringToOffset.Add(str, stringTable.WrittenCount - Encoding.UTF8.GetByteCount(str) - 1);
            }
            else
            {
                lastSymbol = str;
                stringToOffset.Add(str, stringTable.WrittenCount);

                int utf8Length = Encoding.UTF8.GetByteCount(str.AsSpan());
                var utf8Span = stringTable.GetSpan(utf8Length + 1);
                Encoding.UTF8.GetBytes(str, utf8Span);
                utf8Span[utf8Length] = 0;
                stringTable.Advance(utf8Length + 1);
            }
        }

        MultiKeySortByteSize = (ulong)stringTable.WrittenCount;

        static char TailCharacter(string str, int pos) => pos < str.Length ? str[str.Length - pos - 1] : (char)0;

        static void MultiKeySort(Span<string> input, int pos)
        {
            if (!MultiKeySortSmallInput(input, pos))
            {
                MultiKeySortLargeInput(input, pos);
            }
        }

        static void MultiKeySortLargeInput(Span<string> input, int pos)
        {
        tailcall:
            char pivot = TailCharacter(input[0], pos);
            int l = 0, h = input.Length;
            for (int i = 1; i < h;)
            {
                char c = TailCharacter(input[i], pos);
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
            MultiKeySort(input.Slice(h), pos);
            if (pivot != (char)0)
            {
                // Use a loop as a poor man's tailcall
                // MultiKeySort(input.Slice(l, h - l), pos + 1);
                pos++;
                input = input.Slice(l, h - l);
                if (!MultiKeySortSmallInput(input, pos))
                {
                    goto tailcall;
                }
            }
        }

        static bool MultiKeySortSmallInput(Span<string> input, int pos)
        {
            if (input.Length <= 1)
                return true;

            // Optimize comparing two strings
            if (input.Length == 2)
            {
                while (true)
                {
                    char c0 = TailCharacter(input[0], pos);
                    char c1 = TailCharacter(input[1], pos);
                    if (c0 < c1)
                    {
                        (input[0], input[1]) = (input[1], input[0]);
                        break;
                    }
                    else if (c0 > c1 || c0 == (char)0)
                    {
                        break;
                    }
                    pos++;
                }
                return true;
            }

            return false;
        }
    }
}