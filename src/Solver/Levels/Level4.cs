using Solver.Framework;

namespace Solver.Levels;

public record Level4Input(string[] Cases);

public sealed class Level4 : LevelBase<Level4Input>
{
    public override int Number => 4;

    // Default shape: first line = number of cases, then one case per line. Adjust to the level's format.
    public override Level4Input Parse(InputReader reader)
    {
        var count = reader.Int();
        return new Level4Input(reader.Lines(count));
    }

    public override IEnumerable<string> Solve(Level4Input input) =>
        input.Cases.Select(SolveCase);

    private static string SolveCase(string line) =>
        throw new NotImplementedException();
}
