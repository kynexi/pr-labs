namespace Lab1.Common;

// Where the data files are. Paths are relative to lab1/csharp (the folder you run from).
public static class Paths
{
    public static readonly string DataDir = Path.Combine("..", "data");
    public static readonly string NumbersFile = Path.Combine(DataDir, "numbers.txt");

    // part_1.txt, part_2.txt, ... part_8.txt
    public static string PartFile(string dir, int i) => Path.Combine(dir, $"part_{i}.txt");
}
