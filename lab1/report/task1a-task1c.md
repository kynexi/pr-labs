# Tasks 1a and 1c — File partitioning and producer–consumer queue

The generated 50,000,000-number input was read from the internal NVMe disk.
Each configuration was run once in Release mode. The correct reference totals
are 25,005,087 negative, 24,994,889 positive, and 24 zero. Every run returned
these totals. Raw results are in [task1a-measurements.csv](../results/task1a-measurements.csv)
and [task1c-measurements.csv](../results/task1c-measurements.csv).

## Task 1a — Equal file partitions

| Threads | Time (ms) | Speed-up | Peak memory (MB) |
|---:|---:|---:|---:|
| 1 | 533.7 | 1.00x | 25.9 |
| 2 | 320.9 | 1.66x | 25.8 |
| 4 | 182.0 | 2.93x | 25.8 |
| 8 | 128.0 | 4.17x | 26.6 |
| 16 | 115.9 | 4.61x | 26.7 |
| 32 | 117.6 | 4.54x | 28.1 |
| 64 | 118.7 | 4.50x | 30.2 |

Speed-up is the one-thread time divided by the time at each thread count.
Throughput improves substantially up to 8 threads and a little further at 16;
it stops improving at 16 and slightly regresses at 32 and 64. This machine has
8 physical cores and 16 logical processors. The observed plateau at the logical
processor count is consistent with exhausting useful CPU parallelism, with
thread scheduling and memory/disk bandwidth costs outweighing any benefit from
more threads.

Peak memory stays nearly constant because the file is partitioned rather than
loaded into one large array. The small increase with thread count comes from
per-thread buffers and runtime/thread overhead.

## Task 1c — Producer and consumers

| Workers | Time (ms) | Speed-up | Peak memory (MB) |
|---:|---:|---:|---:|
| 1 | 1,552.9 | 1.00x | 194.6 |
| 2 | 1,384.6 | 1.12x | 68.4 |
| 4 | 1,494.4 | 1.04x | 76.9 |
| 8 | 1,992.2 | 0.78x | 118.3 |
| 16 | 1,663.0 | 0.93x | 164.9 |
| 32 | 1,782.8 | 0.87x | 173.6 |
| 64 | 1,988.5 | 0.78x | 189.6 |

Speed-up is relative to the one-worker queue run. Two workers were fastest,
but the improvement was modest (about 1.12x); adding workers did not improve
the result and was often slower. The reader is a single producer and can
become the bottleneck. Compared with Task 1a, Task 1c is much slower: the
single producer reads and decodes each line, allocates batches of strings,
and synchronizes with consumers through the bounded queue. Task 1a instead
lets each thread read and parse its own byte range.

The queue holds at most 100 batches, limiting the amount of queued work.
Memory measurements for Task 1c vary substantially (notably the one-worker
run) because peak process working set includes runtime and managed allocation
behavior, including allocated batches and strings. They are single-run
measurements, not a smooth per-thread memory curve.

With one-line batches, queue operations and producer/consumer synchronization
would occur for every number, greatly increasing overhead. With no queue size
limit, a faster producer could enqueue data faster than consumers process it,
causing unbounded memory growth instead of applying backpressure.

## Machine

- CPU: AMD Ryzen 7 8845HS, 8 physical cores / 16 logical processors
- RAM: 27.8 GiB
- Internal disk: WD PC SN740 NVMe SSD
- OS: Windows 10 Pro (build 26200)
- Runtime: .NET 8.0.412

These are single runs from a local disk; the results show the scaling observed
in this run, not averaged or cold-cache measurements.
