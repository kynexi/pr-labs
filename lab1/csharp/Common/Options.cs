namespace Lab1.Common;

// Reads the command line, for example: run --task 1a --threads 8
// The first word is the command; after it come "--name value" pairs.
public sealed class Options
{
    public const string Usage = """
        usage:
          lab1 generate [--out PATH] [--count 50000000] [--seed 42]
          lab1 split    [--in PATH] [--parts 8] [--outdir DIR]
          lab1 verify   [--file PATH]
          lab1 header
          lab1 run --task 1a|1b|1c|2 [--threads N] [--variant unsync|lock|local]
                   [--file PATH] [--dir DIR]
        """;

    public string Command { get; }
    private readonly Dictionary<string, string> _values = new();

    private Options(string command) => Command = command;

    public static Options Parse(string[] args)
    {
        var o = new Options(args.Length > 0 ? args[0] : "");
        for (int i = 1; i < args.Length; i++)
        {
            if (!args[i].StartsWith("--"))
                throw new ArgumentException($"unexpected argument '{args[i]}'");
            string key = args[i][2..];
            if (i + 1 >= args.Length)
                throw new ArgumentException($"missing value for --{key}");
            o._values[key] = args[++i];
        }
        return o;
    }

    // Each Get returns the value given on the command line, or the default if it was not given.
    public string Get(string key, string fallback) =>
        _values.TryGetValue(key, out var v) ? v : fallback;

    public int GetInt(string key, int fallback) =>
        _values.TryGetValue(key, out var v) ? int.Parse(v) : fallback;

    public long GetLong(string key, long fallback) =>
        _values.TryGetValue(key, out var v) ? long.Parse(v) : fallback;
}
