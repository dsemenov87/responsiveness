```

BenchmarkDotNet v0.14.0, Manjaro Linux
Intel Core i5-6200U CPU 2.30GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.108
  [Host]   : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method | N     | Mean      | Error     | StdDev    | Median    |
|------- |------ |----------:|----------:|----------:|----------:|
| **Sha256** | **1000**  |  **4.961 μs** | **0.2000 μs** | **0.5739 μs** |  **4.705 μs** |
| Md5    | 1000  |  3.652 μs | 0.2031 μs | 0.5925 μs |  3.580 μs |
| **Sha256** | **10000** | **39.697 μs** | **2.3081 μs** | **6.6963 μs** | **36.616 μs** |
| Md5    | 10000 | 24.486 μs | 1.0051 μs | 2.9320 μs | 23.503 μs |
