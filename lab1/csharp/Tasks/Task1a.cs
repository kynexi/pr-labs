using Lab1.Common;

namespace Lab1.Tasks;

// Task 1a: each thread counts its own piece of numbers.txt.
//
// Rule for lines cut in the middle: a line belongs to the thread
// whose piece contains the FIRST character of that line.
//   - a thread skips the partial line at the start of its piece (the previous thread has it)
//   - a thread finishes its last line even if it goes past the end of its piece
public static class Task1a
{
    private const int BufferSize = 64 * 1024;

    public static Counts Run(string file, int threads)
    {
        long length = new FileInfo(file).Length;
        var results = new Counts[threads];
        var workers = new Thread[threads];

        for (int t = 0; t < threads; t++)
        {
            int id = t;
            long start = length * id / threads;
            long end = length * (id + 1) / threads;
            workers[id] = new Thread(() => results[id] = CountPiece(file, start, end));
            workers[id].Start();
        }

        foreach (var w in workers) w.Join();

        var total = new Counts();
        foreach (var r in results) total += r;
        return total;
    }

    private static Counts CountPiece(string file, long start, long end)
    {
        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 1);

        // Start one byte early: if that byte is '\n', our first line starts exactly at `start`.
        long pos = start > 0 ? start - 1 : 0;
        bool skipping = start > 0;
        fs.Position = pos;

        long neg = 0, positive = 0, zero = 0;
        int value = 0;
        bool negative = false;
        bool atLineStart = !skipping;
        bool hasDigits = false;

        var buf = new byte[BufferSize];
        int read;
        while ((read = fs.Read(buf, 0, buf.Length)) > 0)
        {
            for (int i = 0; i < read; i++)
            {
                byte b = buf[i];

                if (skipping)
                {
                    if (b == '\n') { skipping = false; atLineStart = true; }
                    continue;
                }

                if (atLineStart && pos + i >= end)
                    return new Counts(neg, positive, zero); // next line belongs to the next thread

                if (b == '\n')
                {
                    if (hasDigits) Count(negative ? -value : value, ref neg, ref positive, ref zero);
                    value = 0; negative = false; hasDigits = false;
                    atLineStart = true;
                }
                else
                {
                    atLineStart = false;
                    if (b == '-') negative = true;
                    else if (b >= '0' && b <= '9') { value = value * 10 + (b - '0'); hasDigits = true; }
                }
            }
            pos += read;
        }

        if (hasDigits) Count(negative ? -value : value, ref neg, ref positive, ref zero); // last line without '\n'
        return new Counts(neg, positive, zero);
    }

    private static void Count(int n, ref long neg, ref long positive, ref long zero)
    {
        if (n < 0) neg++;
        else if (n > 0) positive++;
        else zero++;
    }
}
