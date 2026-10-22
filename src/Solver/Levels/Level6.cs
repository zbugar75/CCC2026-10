using Solver.Framework;

namespace Solver.Levels;

public record Level6Input(string[] Cases);

public sealed class Level6 : LevelBase<Level6Input>
{
    public override int Number => 6;

    // Default shape: first line = number of cases, then one case per line. Adjust to the level's format.
    public override Level6Input Parse(InputReader reader)
    {
        var count = reader.Int();
        return new Level6Input(reader.Lines(count));
    }

    public override IEnumerable<string> Solve(Level6Input input) =>
        input.Cases.Select(SolveCase);

    private static string SolveCase(string line) =>
        throw new NotImplementedException();
}
