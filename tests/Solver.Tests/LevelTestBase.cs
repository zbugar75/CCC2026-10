using Solver.Framework;

namespace Solver.Tests;

/// <summary>
/// Runs the level's logic on Input/LevelN/*example*.in and compares it with the provided *example*.out.
/// Our result is also written to Output/LevelN/ for inspection.
/// Skipped while the level's example files are not in place yet.
/// </summary>
public abstract class LevelTestBase<TLevel> where TLevel : ILevel, new()
{
    [Fact]
    public void Example_matches_expected_output()
    {
        var level = new TLevel();
        var input = Paths.ExampleIn(level.Number);
        var expectedPath = Paths.ExampleOut(level.Number);
        if (input is null || expectedPath is null)
            Assert.Skip($"No example .in/.out pair in {Paths.InputDir(level.Number)}");

        var actual = Runner.Execute(level, input);

        var outputPath = Paths.OutputFor(level.Number, input);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        File.WriteAllText(outputPath, actual);

        Assert.Equal(Normalize(File.ReadAllText(expectedPath)), Normalize(actual));
    }

    private static string Normalize(string text) =>
        string.Join("\n", text.Replace("\r\n", "\n").Split('\n').Select(l => l.TrimEnd())).Trim();
}
