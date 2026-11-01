using Lab1.Common;

namespace Lab1.Tasks;

// Task 1c: one reader thread and several worker threads.
// The reader puts lines into a queue, in groups of 10 000.
// The workers take groups from the queue and count them.
public static class Task1c
{
    public const int BatchSize = 10_000;  // lines per group
    public const int QueueCapacity = 100; // max groups in the queue

    public static Counts Run(string file, int workers)
    {
        // TODO (Student B): a good fit is BlockingCollection<string[]>(QueueCapacity).
        //   Reader:  queue.Add(group) for each group, then queue.CompleteAdding() at the end.
        //   Workers: foreach (var group in queue.GetConsumingEnumerable()) { count it }
        throw new NotImplementedException("Task 1c is not implemented yet");
    }
}
