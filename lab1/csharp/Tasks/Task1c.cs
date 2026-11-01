using System.Collections.Concurrent;
using Lab1.Common;

namespace Lab1.Tasks;

// Task 1c: one reader thread puts batches of lines into a queue,
// worker threads take batches out and count them.
public static class Task1c
{
    public const int BatchSize = 10_000;
    public const int QueueCapacity = 100;

    public static Counts Run(string file, int workers)
    {
        // Bounded queue: Add() waits when it has 100 batches, Take waits when it is empty.
        using var queue = new BlockingCollection<string[]>(QueueCapacity);

        var reader = new Thread(() =>
        {
            using var sr = new StreamReader(file);
            var batch = new List<string>(BatchSize);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                batch.Add(line);
                if (batch.Count == BatchSize)
                {
                    queue.Add(batch.ToArray());
                    batch.Clear();
                }
            }
            if (batch.Count > 0) queue.Add(batch.ToArray()); // last, smaller batch
            queue.CompleteAdding(); // close the queue
        });

        var results = new Counts[workers];
        var threads = new Thread[workers];
        for (int w = 0; w < workers; w++)
        {
            int id = w;
            threads[id] = new Thread(() =>
            {
                long neg = 0, pos = 0, zero = 0;
                // Ends when the queue is closed AND empty.
                foreach (var b in queue.GetConsumingEnumerable())
                {
                    foreach (var s in b)
                    {
                        int n = int.Parse(s);
                        if (n < 0) neg++;
                        else if (n > 0) pos++;
                        else zero++;
                    }
                }
                results[id] = new Counts(neg, pos, zero);
            });
        }

        reader.Start();
        foreach (var t in threads) t.Start();

        reader.Join();
        foreach (var t in threads) t.Join();

        var total = new Counts();
        foreach (var r in results) total += r;
        return total;
    }
}
