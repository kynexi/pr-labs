namespace Lab1.Common;

// The answer every task returns: how many numbers are negative, positive and zero.
public readonly record struct Counts(long Negative, long Positive, long Zero)
{
    public const long Expected = 50_000_000;

    public long Total => Negative + Positive + Zero;
    public bool IsComplete => Total == Expected; // the lab's correctness check

    // Lets you add two results: total = a + b
    public static Counts operator +(Counts a, Counts b) =>
        new(a.Negative + b.Negative, a.Positive + b.Positive, a.Zero + b.Zero);
}
