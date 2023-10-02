# Benchmark for string table building algorithms

- **Naive** produces string table with no suffix deduplication
- **MultiKeySort** sorts the input strings and then produces optimized string table with deduplicated suffixes
- **ElfStringTable** uses LibObjectFile's `ElfStringTable`, which incrementally builds the table and uses dictionary to deduplicate suffixes

Results on MacBook Air M1:
| Method         | Mean       | Error     | StdDev    | Gen0       | Gen1       | Gen2      | Allocated |
|--------------- |-----------:|----------:|----------:|-----------:|-----------:|----------:|----------:|
| ElfStringTable | 352.150 ms | 2.8391 ms | 2.5168 ms | 38000.0000 | 21000.0000 | 7000.0000 |  316.8 MB |
| MultiKeySort   |   7.985 ms | 0.1563 ms | 0.2655 ms |  1062.5000 |  1046.8750 |  968.7500 |   4.87 MB |
| Naive          |   2.390 ms | 0.0097 ms | 0.0086 ms |  1042.9688 |  1015.6250 |  996.0938 |   5.82 MB |
