using System.Diagnostics;
using Lab1;
using Lab1.Common;
using Lab1.Tasks;

// The program starts here.
// The first word says what to do:
//   generate  - create numbers.txt
//   split     - cut numbers.txt into 8 files
//   verify    - check numbers.txt and print the correct totals
//   header    - print the CSV column names
//   run       - run one task, for example: run --task 1a --threads 8

var opts = Options.Parse(args);

try
{
    switch (opts.Command)
    {
        case "generate":
            DataGenerator.Generate(
                opts.Get("out", Paths.NumbersFile),
                opts.GetLong("count", 50_000_000),
                opts.GetInt("seed", 42));
            return 0;

        case "split":
            DataGenerator.Split(
                opts.Get("in", Paths.NumbersFile),
                opts.GetInt("parts", 8),
                opts.Get("outdir", Paths.DataDir));
            return 0;

        case "verify":
            DataGenerator.Verify(opts.Get("file", Paths.NumbersFile));
            return 0;

        case "header":
            Console.WriteLine(Result.CsvHeader);
            return 0;

        case "run":
            return Run(opts);

        default:
            Console.Error.WriteLine(Options.Usage);
            return 1;
    }
}
catch (Exception e) when (e is ArgumentException or FileNotFoundException
                               or DirectoryNotFoundException or NotImplementedException)
{
    Console.Error.WriteLine($"error: {e.Message}");
    return 1;
}

// Runs one task, measures time and memory, prints one CSV line.
static int Run(Options opts)
{
    string task = opts.Get("task", "");
    int threads = opts.GetInt("threads", Environment.ProcessorCount);
    string variant = opts.Get("variant", "-");
    string file = opts.Get("file", Paths.NumbersFile);
    string dir = opts.Get("dir", Paths.DataDir);

    // Task 1b: loading the numbers is not timed, so we do it before starting the timer.
    int[]? preloaded = task == "1b" ? Task1b.LoadAll(file) : null;

    var sw = Stopwatch.StartNew();
    Counts counts = task switch
    {
        "1a" => Task1a.Run(file, threads),
        "1b" => Task1b.Run(preloaded!, threads, variant),
        "1c" => Task1c.Run(file, threads),
        "2"  => Task2.Run(dir, threads),
        _    => throw new ArgumentException($"unknown task '{task}' (expected 1a, 1b, 1c or 2)"),
    };
    sw.Stop();

    var result = new Result(task, variant, threads, sw.Elapsed.TotalMilliseconds,
                            PeakMemory.Megabytes(), counts);
    Console.WriteLine(result.ToCsv());

    if (!counts.IsComplete)
        Console.Error.WriteLine($"WARNING: total = {counts.Total:N0}, expected {Counts.Expected:N0}");
    return 0;
}
