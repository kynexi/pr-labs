using System.Buffers.Text;
using System.Diagnostics;
using Lab1.Common;

namespace Lab1;

// Part 2 of the lab ("Data"): create numbers.txt and split it into 8 files.
// This is preparation only; it is not measured.
public static class DataGenerator
{
    private const int Min = -1_000_000;
    private const int Max = 1_000_000;
    private const int BufferSize = 1 << 20; // 1 MB

    // Writes `count` random numbers between -1 000 000 and 1 000 000, one per line.
    public static void Generate(string path, long count, int seed)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        var sw = Stopwatch.StartNew();

        // Same seed = same numbers every time, so both of us get the exact same file.
        var rng = new Random(seed);

        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize);
        var buf = new byte[BufferSize];
        int pos = 0;

        for (long i = 0; i < count; i++)
        {
            // Buffer almost full: write it to the file and start again.
            if (pos > buf.Length - 16) { fs.Write(buf, 0, pos); pos = 0; }

            int n = rng.Next(Min, Max + 1); // +1 because Next() never returns the upper bound
            Utf8Formatter.TryFormat(n, buf.AsSpan(pos), out int written); // number -> text
            pos += written;
            buf[pos++] = (byte)'\n';
        }
        fs.Write(buf, 0, pos);

        Console.WriteLine($"generated {count:N0} numbers -> {path} " +
                          $"({fs.Length / (1024.0 * 1024):F1} MB) in {sw.Elapsed.TotalSeconds:F1} s");
    }

    // Cuts the file, in order, into `parts` files with the same number of lines.
    // 50 000 000 lines / 8 files = 6 250 000 lines per file.
    public static void Split(string input, int parts, string outDir)
    {
        if (!File.Exists(input)) throw new FileNotFoundException($"input not found: {input}");
        Directory.CreateDirectory(outDir);
        var sw = Stopwatch.StartNew();

        long totalLines = CountLines(input);
        if (totalLines % parts != 0)
            throw new ArgumentException($"{totalLines:N0} lines cannot be split evenly into {parts} files");
        long linesPerPart = totalLines / parts;

        using var src = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize);
        var buf = new byte[BufferSize];

        for (int p = 1; p <= parts; p++)
        {
            string outPath = Paths.PartFile(outDir, p);
            using var dst = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize);
            long linesLeft = linesPerPart;

            while (linesLeft > 0)
            {
                // Read a block of the file.
                long start = src.Position;
                int read = src.Read(buf, 0, buf.Length);
                if (read == 0) throw new InvalidDataException("unexpected end of file while splitting");

                // Count newlines. If this part ends inside the block, stop at that newline.
                int take = read;
                for (int i = 0; i < read; i++)
                {
                    if (buf[i] == (byte)'\n' && --linesLeft == 0) { take = i + 1; break; }
                }
                dst.Write(buf, 0, take);

                // The rest of the block belongs to the next part, so go back to where we stopped.
                src.Position = start + take;
            }
            Console.WriteLine($"  {outPath}: {linesPerPart:N0} lines, {dst.Length / (1024.0 * 1024):F1} MB");
        }
        Console.WriteLine($"split into {parts} files in {sw.Elapsed.TotalSeconds:F1} s");
    }

    // Checks the file and prints the correct totals.
    // Every task must later print exactly these numbers.
    public static void Verify(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException($"file not found: {path}");
        long lines = 0, neg = 0, pos = 0, zero = 0;
        int min = int.MaxValue, max = int.MinValue;

        foreach (var line in File.ReadLines(path))
        {
            int n = int.Parse(line);
            lines++;
            if (n < min) min = n;
            if (n > max) max = n;
            if (n < 0) neg++; else if (n > 0) pos++; else zero++;
        }

        Console.WriteLine($"lines={lines:N0} min={min:N0} max={max:N0}");
        Console.WriteLine($"neg={neg} pos={pos} zero={zero} total={neg + pos + zero}");
        Console.WriteLine(lines == Counts.Expected
            ? "OK: write these totals down, every task must get the same"
            : $"WARNING: expected {Counts.Expected:N0} lines");
    }

    // Counts lines by counting '\n' characters.
    private static long CountLines(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize);
        var buf = new byte[BufferSize];
        long lines = 0;
        int read;
        while ((read = fs.Read(buf, 0, buf.Length)) > 0)
            lines += buf.AsSpan(0, read).Count((byte)'\n');
        return lines;
    }
}
