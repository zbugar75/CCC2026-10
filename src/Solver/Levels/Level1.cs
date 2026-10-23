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
        var rad = int.Parse(line);
        var result = PrintNano(rad);
        return result;
    }

    private static string PrintNano(int rad)
    {
        var result = new string[2 * rad + 1];
        for (var i = -rad; i <= rad; i++)
        {
            result[i + rad] = GeneratedRadLine(rad, i);
        }
        return string.Join("\n", result);
    }

    private static string GeneratedRadLine(int rad, int i)
    {
        if (Math.Abs(i) == rad)
        {
            // rad x - + (rad+1) * with dash
            return Dashes(rad) + AlternatingPattern('*', '-', rad + 1) + Dashes(rad);
        }

        // i* - / * - (rad - 1)
        return Dashes(Math.Abs(i)) + "*-" + Places(2 * rad - Math.Abs(i) - 1) + "-*" + Dashes(Math.Abs(i));
    }

    private static string Dashes(int i)
    {
        return new string('-', i);
    }

    private static string Places(int i)
    {
        return AlternatingPattern('O', '-', i);
    }

    static string AlternatingPattern(char c1, char c2, int n) =>
        n <= 0 ? "" : string.Concat(Enumerable.Repeat($"{c1}{c2}", n - 1)) + c1;
}
