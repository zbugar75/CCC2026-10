using Solver.Framework;

namespace Solver.Levels;

public record Level7Input(string[] Cases);

public sealed class Level7 : LevelBase<Level7Input>
{
    public override int Number => 7;

    // Default shape: first line = number of cases, then one case per line. Adjust to the level's format.
    public override Level7Input Parse(InputReader reader)
    {
        var count = reader.Int();
        return new Level7Input(reader.Lines(count));
    }

    public override IEnumerable<string> Solve(Level7Input input) =>
        input.Cases.Select(SolveCase);

    private static string SolveCase(string line) =>
        throw new NotImplementedException();
}
