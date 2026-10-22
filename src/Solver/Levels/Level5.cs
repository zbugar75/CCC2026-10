using Solver.Framework;

namespace Solver.Levels;

public record Level5Input(string[] Cases);

public sealed class Level5 : LevelBase<Level5Input>
{
    public override int Number => 5;

    // Default shape: first line = number of cases, then one case per line. Adjust to the level's format.
    public override Level5Input Parse(InputReader reader)
    {
        var count = reader.Int();
        return new Level5Input(reader.Lines(count));
    }

    public override IEnumerable<string> Solve(Level5Input input) =>
        input.Cases.Select(SolveCase);

    private static string SolveCase(string line) =>
        throw new NotImplementedException();
}
