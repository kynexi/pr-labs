using System.Globalization;

namespace Lab1.Common;

// One run of a task, printed as one CSV line.
// All tasks print the same columns, so the results are easy to put in a table or chart.
public sealed record Result(string Task, string Variant, int Threads, double TimeMs,
                            double PeakMemMb, Counts Counts)
{
    public const string CsvHeader = "task,variant,threads,time_ms,peak_mem_mb,neg,pos,zero,total,ok";

    public string ToCsv() => string.Join(",",
        Task, Variant, Threads,
        TimeMs.ToString("F1", CultureInfo.InvariantCulture),
        PeakMemMb.ToString("F1", CultureInfo.InvariantCulture),
        Counts.Negative, Counts.Positive, Counts.Zero, Counts.Total,
        Counts.IsComplete ? "yes" : "NO");
}
