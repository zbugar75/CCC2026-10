using Solver.Framework;

namespace Solver.Levels;

public record Level2Input(Level2Case[] Cases);

public readonly struct Level2Case(int radius, int rows, int columns)
{
    public int Radius { get; } = radius;
    public int Rows { get; } = rows;

    public int Columns { get; } = columns;

    public string[] Grid { get; } = new string[rows];
}

public sealed class Level2 : LevelBase<Level2Input>
{
    public override int Number => 2;

    // First line = number of cases; each case = "radius rows columns" line, then `rows` grid lines;
    // cases are separated by blank lines.
    public override Level2Input Parse(InputReader reader)
    {
        var count = reader.Int();
        var cases = new Level2Case[count];
        for (var i = 0; i < count; i++)
        {
            reader.SkipBlank();
            var h = reader.Ints(); // radius rows columns
            var c = new Level2Case(h[0], h[1], h[2]);
            for (var r = 0; r < c.Rows; r++)
                c.Grid[r] = reader.Line();
            cases[i] = c;
        }
        return new Level2Input(cases);
    }

    public override IEnumerable<string> Solve(Level2Input input) =>
        input.Cases.Select(SolveCase);


    private static string SolveCase(Level2Case c)
    {
        var result = PrintNano(c);
        return result;
    }

    private static string PrintNano(Level2Case level2Case)
    {
        var emitters = CollectEmitters(level2Case);
        emitters.Select(e => ApplyEmitter(level2Case, e));
        return string.Join("\n", level2Case.Grid);
    }

    //private static string GeneratedRadLine(int rad, int i)
    //{
    //    if (Math.Abs(i) == rad)
    //    {
    //        // rad x - + (rad+1) * with dash
    //        return Dashes(rad) + AlternatingPattern('*', '-', rad + 1) + Dashes(rad);
    //    }

    //    // i* - / * - (rad - 1)
    //    return Dashes(Math.Abs(i)) + "*-" + Places(2 * rad - Math.Abs(i) - 1) + "-*" + Dashes(Math.Abs(i));
    //}

    //private static string Dashes(int i)
    //{
    //    return new string('-', i);
    //}

    //private static string Places(int i)
    //{
    //    return AlternatingPattern('O', '-', i);
    //}

    //static string AlternatingPattern(char c1, char c2, int n) =>
    //    n <= 0 ? "" : string.Concat(Enumerable.Repeat($"{c1}{c2}", n - 1)) + c1;
}
