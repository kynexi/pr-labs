using Lab1.Common;

namespace Lab1.Tasks;

// Task 1b: count the numbers from an array, in three different ways.
//   --variant unsync : all threads add to the same counters, no lock (gives wrong totals)
//   --variant lock   : all threads add to the same counters, with a lock
//   --variant local  : each thread has its own counters, added together at the end
public static class Task1b
{
    // Reads all numbers into an array. This part is not timed.
    public static int[] LoadAll(string file)
    {
        if (!File.Exists(file)) throw new FileNotFoundException($"input not found: {file}");

        var numbers = new int[(int)Counts.Expected];
        int count = 0;
        foreach (string line in File.ReadLines(file))
        {
            if (count == numbers.Length) Array.Resize(ref numbers, checked(numbers.Length * 2));
            numbers[count++] = int.Parse(line);
        }

        if (count != numbers.Length) Array.Resize(ref numbers, count);
        return numbers;
    }

    public static Counts Run(int[] numbers, int threads, string variant)
    {
        ArgumentNullException.ThrowIfNull(numbers);
        if (threads <= 0) throw new ArgumentOutOfRangeException(nameof(threads), "thread count must be positive");

        return variant switch
        {
            "unsync" => CountUnsynchronized(numbers, threads),
            "lock"   => CountWithLock(numbers, threads),
            "local"  => CountLocally(numbers, threads),
            _ => throw new ArgumentException($"unknown variant '{variant}' (expected unsync, lock or local)"),
        };
    }

    private static Counts CountUnsynchronized(int[] numbers, int threads)
    {
        long negative = 0, positive = 0, zero = 0;
        var workers = new Thread[threads];
        StartWorkers(numbers, workers, (start, end) =>
        {
            for (int i = start; i < end; i++)
            {
                int number = numbers[i];
                if (number < 0) negative++;
                else if (number > 0) positive++;
                else zero++;
            }
        });
        JoinWorkers(workers);
        return new Counts(negative, positive, zero);
    }

    private static Counts CountWithLock(int[] numbers, int threads)
    {
        long negative = 0, positive = 0, zero = 0;
        var gate = new object();
        var workers = new Thread[threads];
        StartWorkers(numbers, workers, (start, end) =>
        {
            for (int i = start; i < end; i++)
            {
                int number = numbers[i];
                lock (gate)
                {
                    if (number < 0) negative++;
                    else if (number > 0) positive++;
                    else zero++;
                }
            }
        });
        JoinWorkers(workers);
        return new Counts(negative, positive, zero);
    }

    private static Counts CountLocally(int[] numbers, int threads)
    {
        var counts = new Counts[threads];
        var workers = new Thread[threads];
        for (int worker = 0; worker < threads; worker++)
        {
            int index = worker;
            GetRange(numbers.Length, threads, index, out int start, out int end);
            workers[index] = new Thread(() =>
            {
                long negative = 0, positive = 0, zero = 0;
                for (int i = start; i < end; i++)
                {
                    int number = numbers[i];
                    if (number < 0) negative++;
                    else if (number > 0) positive++;
                    else zero++;
                }
                counts[index] = new Counts(negative, positive, zero);
            });
            workers[index].Start();
        }

        JoinWorkers(workers);
        Counts total = default;
        foreach (Counts count in counts) total += count;
        return total;
    }

    private static void StartWorkers(int[] numbers, Thread[] workers, Action<int, int> countRange)
    {
        for (int worker = 0; worker < workers.Length; worker++)
        {
            GetRange(numbers.Length, workers.Length, worker, out int start, out int end);
            workers[worker] = new Thread(() => countRange(start, end));
            workers[worker].Start();
        }
    }

    private static void GetRange(int length, int workers, int worker, out int start, out int end)
    {
        int baseSize = length / workers;
        int remainder = length % workers;
        start = worker * baseSize + Math.Min(worker, remainder);
        end = start + baseSize + (worker < remainder ? 1 : 0);
    }

    private static void JoinWorkers(Thread[] workers)
    {
        foreach (Thread worker in workers) worker.Join();
    }
}
