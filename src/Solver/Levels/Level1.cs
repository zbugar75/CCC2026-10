using Solver.Framework;

namespace Solver.Levels;

public record Level1Input(string[] Cases);

public sealed class Level1 : LevelBase<Level1Input>
{
    public override int Number => 1;

    // Default shape: first line = number of cases, then one case per line. Adjust to the level's format.
    public override Level1Input Parse(InputReader reader)
    {
        var count = reader.Int();
        return new Level1Input(reader.Lines(count));
    }

    public override IEnumerable<string> Solve(Level1Input input) =>
        input.Cases.Select(SolveCase);

    private static string SolveCase(string line)
    {
        var rad = line.Cast<int>;
        string result = PrintNano(line);
        return result;
    }

    private static string PrintNano(string line)
    {
        return null;
    }
}
