using System.Diagnostics;

namespace Solver.Framework;

public static class Runner
{
    /// <summary>
    /// Runs the level on every Input/LevelN/in_*.txt file (example first) and writes Output/LevelN/out_*.txt.
    /// A failure in one file is reported and the remaining files still run.
    /// </summary>
    public static void Run(ILevel level)
    {
        var inputDir = Paths.InputDir(level.Number);
        var inputs = Directory.Exists(inputDir)
            ? Directory.GetFiles(inputDir, Paths.InputPattern)
                .OrderBy(f => Path.GetFileName(f).Contains("example") ? 0 : 1)
                .ThenBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToArray()
            : [];

        if (inputs.Length == 0)
        {
            Console.WriteLine($"No {Paths.InputPattern} files in {inputDir}");
            return;
        }

        Console.WriteLine($"Level {level.Number}: {inputs.Length} input file(s)");

        foreach (var input in inputs)
        {
            var name = Path.GetFileName(input);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var output = Execute(level, input);
                var outputPath = Paths.OutputFor(level.Number, input);
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
                File.WriteAllText(outputPath, output);
                Console.WriteLine($"  OK    {name} -> {Path.GetFileName(outputPath)} ({stopwatch.ElapsedMilliseconds} ms)");
            }
            catch (Exception e)
            {
                Console.WriteLine($"  FAIL  {name} ({stopwatch.ElapsedMilliseconds} ms): {e.GetType().Name}: {e.Message}");
            }
        }
    }

    /// <summary>Runs the level on one input file and returns the output text.</summary>
    public static string Execute(ILevel level, string inputPath) =>
        string.Join("\n\n", level.Run(InputReader.FromFile(inputPath))) + "\n";
}
