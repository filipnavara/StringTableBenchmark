# Benchmark for string table building algorithms

| Method         | Mean       | Error     | StdDev    | Gen0       | Gen1       | Gen2      | Allocated |
|--------------- |-----------:|----------:|----------:|-----------:|-----------:|----------:|----------:|
| ElfStringTable | 350.838 ms | 6.8811 ms | 8.1914 ms | 38000.0000 | 21000.0000 | 7000.0000 | 316.81 MB |
| MultiKeySort   |  11.456 ms | 0.0469 ms | 0.0439 ms |  1062.5000 |  1046.8750 |  968.7500 |   4.87 MB |
| Naive          |   2.374 ms | 0.0060 ms | 0.0050 ms |  1042.9688 |  1015.6250 |  996.0938 |   5.82 MB |

**Naive** produces string table with no suffix deduplication
**MultiKeySort** sorts the input strings and then produces optimized string table with deduplicated suffixes
**ElfStringTable** uses LibObjectFile's `ElfStringTable`, which incrementally builds the table and uses dictionary to deduplicate suffixes