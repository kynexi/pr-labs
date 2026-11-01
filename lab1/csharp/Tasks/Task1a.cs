using Lab1.Common;

namespace Lab1.Tasks;

// Task 1a: count the numbers in numbers.txt using several threads.
// Each thread reads only its own piece of the file.
public static class Task1a
{
    public static Counts Run(string file, int threads)
    {
        // TODO (Student A):
        // 1. Cut the file into `threads` pieces of about equal size (in bytes).
        // 2. A piece usually starts or ends in the middle of a line. Decide which thread
        //    counts that line, so no line is counted twice or skipped.
        // 3. Each thread opens the file, reads its piece, and counts.
        // 4. Wait for all threads, then add their results together.
        throw new NotImplementedException("Task 1a is not implemented yet");
    }
}
