using Solver.Framework;

namespace Solver.Levels;

public record Level2Input(string[] Cases);

public sealed class Level2 : LevelBase<Level2Input>
{
    public override int Number => 2;

    // Default shape: first line = number of cases, then one case per line. Adjust to the level's format.
    public override Level2Input Parse(InputReader reader)
    {
        var count = reader.Int();
        return new Level2Input(reader.Lines(count));
    }

    public override IEnumerable<string> Solve(Level2Input input) =>
        input.Cases.Select(SolveCase);

    private static string SolveCase(string line) =>
        throw new NotImplementedException();
}
