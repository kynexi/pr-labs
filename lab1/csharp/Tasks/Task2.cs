using System.Buffers.Text;
using System.Runtime.ExceptionServices;
using Lab1.Common;

namespace Lab1.Tasks;

// Task 2: count the numbers in the 8 files on the USB drive.
// Each thread reads whole files at once (File.ReadAllBytes) and counts them.
// Run it with --dir pointing at the USB drive, for example --dir /Volumes/USB
public static class Task2
{
    public const int FileCount = 8;

    public static Counts Run(string dir, int threads)
    {
        if (threads <= 0) throw new ArgumentOutOfRangeException(nameof(threads), "thread count must be positive");
        if (!Directory.Exists(dir)) throw new DirectoryNotFoundException($"input directory not found: {dir}");

        var partialCounts = new Counts[threads];
        var errors = new ExceptionDispatchInfo?[threads];
        var workers = new Thread[threads];

        for (int worker = 0; worker < threads; worker++)
        {
            int index = worker;
            int start = worker * FileCount / threads;
            int end = (worker + 1) * FileCount / threads;
            workers[index] = new Thread(() =>
            {
                try
                {
                    Counts total = default;
                    for (int part = start; part < end; part++)
                        total += CountFile(Paths.PartFile(dir, part + 1));
                    partialCounts[index] = total;
                }
                catch (Exception e)
                {
                    errors[index] = ExceptionDispatchInfo.Capture(e);
                }
            });
            workers[index].Start();
        }

        foreach (Thread worker in workers) worker.Join();
        foreach (ExceptionDispatchInfo? error in errors)
            error?.Throw();

        Counts result = default;
        foreach (Counts count in partialCounts) result += count;
        return result;
    }

    private static Counts CountFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        long negative = 0, positive = 0, zero = 0, lineNumber = 0;
        int lineStart = 0;

        for (int position = 0; position <= bytes.Length; position++)
        {
            if (position != bytes.Length && bytes[position] != (byte)'\n') continue;

            int lineLength = position - lineStart;
            if (lineLength > 0 && bytes[lineStart + lineLength - 1] == (byte)'\r')
                lineLength--;

            if (lineLength > 0)
            {
                lineNumber++;
                if (!Utf8Parser.TryParse(bytes.AsSpan(lineStart, lineLength), out int number, out int consumed)
                    || consumed != lineLength)
                    throw new InvalidDataException($"invalid integer in {path} at line {lineNumber}");

                if (number < 0) negative++;
                else if (number > 0) positive++;
                else zero++;
            }

            lineStart = position + 1;
        }

        return new Counts(negative, positive, zero);
    }
}
