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
        // TODO (Student A): share the 8 files between the threads as evenly as possible.
        throw new NotImplementedException("Task 2 is not implemented yet");
    }
}
