# Task 2 — Many files on slow storage

The eight input files were split in order from the generated 50,000,000-number
file and copied to a Kingston DataTraveler 3.0 USB drive. Each worker reads
each assigned file in one `File.ReadAllBytes` operation, parses the byte buffer,
and counts signs. Files are assigned in contiguous groups with group sizes
differing by at most one.

## Measurements

The drive was safely ejected and physically unplugged/reconnected before each
configuration to avoid serving the file contents from the operating-system
cache. One run was made per configuration, as allowed by the lab. Raw output
is in [task2.csv](../results/task2.csv).

| Threads | Time (ms) | Speed-up | Peak memory (MB) |   Negative |   Positive | Zero |      Total | Correct |
| ------: | --------: | -------: | ---------------: | ---------: | ---------: | ---: | ---------: | ------- |
|       1 |   4,144.3 |    1.00x |             69.5 | 25,005,087 | 24,994,889 |   24 | 50,000,000 | Yes     |
|       2 |   3,665.8 |    1.13x |            113.6 | 25,005,087 | 24,994,889 |   24 | 50,000,000 | Yes     |
|       4 |   3,641.7 |    1.14x |            246.7 | 25,005,087 | 24,994,889 |   24 | 50,000,000 | Yes     |

Speed-up is the one-worker time divided by the time for that worker count.
All runs produced the reference totals from `verify`.

## Answers

The required comparison with Task 1a cannot yet be made: `Task1a.Run` is still
unimplemented, so there is no one-thread internal-disk baseline. The USB
one-thread time was 4,144.3 ms. Measure Task 1a on the same input before
concluding quantitatively how much slower the USB is or classifying the
workload by comparison.

On the USB, two workers improved elapsed time by about 11.6%; four workers
improved it by about 12.1% versus one. Most of the gain was reached at two
workers, with little additional improvement at four. This is consistent with
the USB read throughput becoming the bottleneck: once the drive is supplying
data near its limit, extra readers cannot raise that limit. The modest gain
does show some overlap of reads and parsing, but these single measurements
alone do not establish the Task 1a comparison.

Peak memory increases as workers increase because each worker reads an entire
file into its own byte buffer. Four workers can hold several roughly 44 MB
buffers concurrently, whereas one worker holds only one at a time.

## Machine and storage

- CPU: AMD Ryzen 7 8845HS, 8 physical cores / 16 logical processors
- RAM: 27.8 GiB
- Internal disk: WD PC SN740 NVMe SSD
- USB drive: Kingston DataTraveler 3.0, approximately 64 GB, mounted as `K:`
- OS: Windows 11 Pro (build 26200)
- Runtime: .NET 8.0.412

The drive reports USB as its bus type and is identified as a DataTraveler 3.0.
Windows lists AMD USB 3.10 host controllers. The exact negotiated link speed
was not confirmed.
