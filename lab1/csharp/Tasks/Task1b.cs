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
        // TODO (Student B)
        throw new NotImplementedException("Task 1b LoadAll is not implemented yet");
    }

    public static Counts Run(int[] numbers, int threads, string variant)
    {
        // TODO (Student B): give each thread a part of the array.
        return variant switch
        {
            "unsync" => throw new NotImplementedException("Task 1b unsync"),
            "lock"   => throw new NotImplementedException("Task 1b lock"),
            "local"  => throw new NotImplementedException("Task 1b local"),
            _ => throw new ArgumentException($"unknown variant '{variant}' (expected unsync, lock or local)"),
        };
    }
}
