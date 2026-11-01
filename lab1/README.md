# Lab 1 — Concurrency in Practice (C#)

## Layout

```
lab1/
  csharp/        the program (one console app, everything chosen by CLI parameters)
    Common/      shared code: CLI options, Counts, CSV Result, peak memory
    Tasks/       Task1a, Task1b, Task1c, Task2
  data/          numbers.txt + part_1..8.txt  (git-ignored, ~400 MB — regenerate)
  results/       CSV results and charts (committed)
  scripts/       benchmark runner and chart scripts
  report/        report + hardware.md
```

## Requirements

.NET SDK 8 or newer (`dotnet --version`).

## Build

```bash
cd lab1/csharp
dotnet build -c Release
```

Always measure with `-c Release`.

## Generate the data

```bash
cd lab1/csharp
dotnet run -c Release -- generate          # ../data/numbers.txt, 50 000 000 numbers, seed 42
dotnet run -c Release -- split             # ../data/part_1.txt ... part_8.txt
dotnet run -c Release -- verify            # line count, min/max, reference totals
```

The seed is fixed, so both of us get the identical file and the same reference totals.

## Run a task

Each run prints one CSV line: `task,variant,threads,time_ms,peak_mem_mb,neg,pos,zero,total,ok`

```bash
dotnet run -c Release -- header
dotnet run -c Release -- run --task 1a --threads 8
dotnet run -c Release -- run --task 1b --variant unsync     # also: lock, local
dotnet run -c Release -- run --task 1c --threads 4
dotnet run -c Release -- run --task 2  --threads 2 --dir /Volumes/USB
```

Task 1b loads `numbers.txt` into memory before starting the timer. Its default
thread count is the number of logical processors; set `--threads` to override
it. Run `unsync` three times and `lock` and `local` once each to collect the
required measurements. The recorded measurements and answers are in
[report/task1b.md](report/task1b.md).

Task 1a reads disjoint file ranges, while Task 1c uses a bounded producer–
consumer queue. Their benchmark results and analysis are in
[report/task1a-task1c.md](report/task1a-task1c.md).

Task 2 reads each assigned file into one byte buffer. Copy the eight split
files to the USB drive, then pass that drive directory with `--dir`; eject and
reconnect the drive before each timed run to avoid cached reads. The
implementation notes and current measurement status are in
[report/task2.md](report/task2.md).

For benchmarks, run the built binary directly (avoids `dotnet run` overhead):
`./bin/Release/net8.0/lab1 run --task 1a --threads 8`
