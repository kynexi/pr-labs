# Task 1b — Three ways to count

The program loaded the 50,000,000 integers into an array before starting the
timer. Each of the 16 worker threads counted a disjoint, nearly equal array
range. The unsynchronized version was run three times; all other versions
were run once. The complete CSV output is in [task1b.csv](../results/task1b.csv).

## Measurements

| Variant | Run | Time (ms) | Peak memory (MB) | Negative | Positive | Zero | Total | Correct |
|---|---:|---:|---:|---:|---:|---:|---:|---|
| Shared, unsynchronized | 1 | 108.2 | 230.3 | 4,014,923 | 3,578,714 | 24 | 7,593,661 | No |
| Shared, unsynchronized | 2 | 105.9 | 230.3 | 3,531,733 | 3,139,982 | 24 | 6,671,739 | No |
| Shared, unsynchronized | 3 | 104.5 | 230.9 | 3,586,904 | 3,183,801 | 24 | 6,770,729 | No |
| Shared, locked | 1 | 3,095.2 | 230.4 | 25,005,087 | 24,994,889 | 24 | 50,000,000 | Yes |
| Per-thread counters | 1 | 29.3 | 230.9 | 25,005,087 | 24,994,889 | 24 | 50,000,000 | Yes |

Reference totals from `verify`: 25,005,087 negative, 24,994,889 positive,
and 24 zero. The same correct totals were produced by the locked and
per-thread versions.

## Answers

The speed ranking in this run is unsynchronized, per-thread, then locked.
The fastest result is invalid: threads concurrently read and update the same
counter, so one thread can overwrite another thread's increment. The unsynchronized
totals vary between runs and omit most numbers.

The locked version is correct because each counter update is protected, but
every number must acquire the same lock. That serializes the workers and makes
it about 106 times slower than per-thread counters here. Per-thread counters
avoid shared updates during counting; the main thread adds the 16 results
after joining the workers, so this version is both correct and fastest of the
correct versions.

Peak memory stayed around 230 MB in all five runs. The loaded integer array
dominates memory use; the counter storage and thread bookkeeping are small in
comparison. Loading is excluded from the reported time, but the process peak
memory includes it.

## Machine and reproduction

- CPU: AMD Ryzen 7 8845HS, 8 physical cores / 16 logical processors
- RAM: 27.8 GiB
- Disk: WD PC SN740 NVMe SSD
- OS: Windows 10 Pro (build 26200)
- Runtime: .NET 8.0.412

From `lab1/csharp`, run `dotnet run -c Release -- run --task 1b
--variant unsync --threads 16` three times, then repeat with `lock` and
`local`. The input was generated with the default seed (`42`).
