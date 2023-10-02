# Benchmark for string table building algorithms

| Method         | Mean      | Error    | StdDev   | Gen0       | Gen1       | Gen2      | Allocated |
|--------------- |----------:|---------:|---------:|-----------:|-----------:|----------:|----------:|
| ElfStringTable | 348.48 ms | 2.503 ms | 2.341 ms | 38000.0000 | 21000.0000 | 7000.0000 | 316.81 MB |
| MultiKeySort   |  15.65 ms | 0.303 ms | 0.337 ms |  1062.5000 |  1046.8750 |  968.7500 |   4.87 MB |
