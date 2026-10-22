namespace Solver.Framework;

/// <summary>
/// Locates the Input/ and Output/ folders by walking up from the binary directory to the repo root.
/// </summary>
public static class Paths
{
    private static readonly Lazy<string> Root = new(FindRoot);

    public static string RepoRoot => Root.Value;

    public static string InputDir(int level) => Path.Combine(RepoRoot, "Input", $"Level{level}");

    public static string OutputDir(int level) => Path.Combine(RepoRoot, "Output", $"Level{level}");

    /// <summary>The level's example input (any *example*.in file), or null if not present yet.</summary>
    public static string? ExampleIn(int level) =>
        Directory.Exists(InputDir(level))
            ? Directory.GetFiles(InputDir(level), "*example*.in").FirstOrDefault()
            : null;

    /// <summary>The provided expected output for the example input, or null if not present.</summary>
    public static string? ExampleOut(int level)
    {
        var input = ExampleIn(level);
        if (input is null) return null;
        var expected = Path.ChangeExtension(input, ".out");
        return File.Exists(expected) ? expected : null;
    }

    /// <summary>Where our result for the given input file is written.</summary>
    public static string OutputFor(int level, string inputPath) =>
        Path.Combine(OutputDir(level), Path.GetFileNameWithoutExtension(inputPath) + ".out");

    private static string FindRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "Input")))
                return dir.FullName;
        }

        throw new DirectoryNotFoundException($"No 'Input' folder found above {AppContext.BaseDirectory}.");
    }
}
