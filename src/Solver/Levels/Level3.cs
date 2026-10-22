using Solver.Framework;

namespace Solver.Levels;

public record Level3Input(string[] Cases);

public sealed class Level3 : LevelBase<Level3Input>
{
    public override int Number => 3;

    // Default shape: first line = number of cases, then one case per line. Adjust to the level's format.
    public override Level3Input Parse(InputReader reader)
    {
        var count = reader.Int();
        return new Level3Input(reader.Lines(count));
    }

    public override IEnumerable<string> Solve(Level3Input input) =>
        input.Cases.Select(SolveCase);

    private static string SolveCase(string line) =>
        throw new NotImplementedException();
}
