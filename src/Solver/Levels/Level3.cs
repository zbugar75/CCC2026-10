using Solver.Framework;

namespace Solver.Levels;

public record Level3Input(Level3Case[] Cases);

public readonly struct Level3Case(int radius, int rows, int columns)
{
    public int Radius { get; } = radius;
    public int Rows { get; } = rows;

    public int Columns { get; } = columns;

    public string[] Grid { get; } = new string[rows];
}

public readonly record struct Level3Emitter(int Row, int Column, int Number);

public sealed class Level3 : LevelBase<Level3Input>
{
    public override int Number => 3;

    // First line = number of cases; each case = "radius rows columns" line, then `rows` grid lines;
    // cases are separated by blank lines.
    public override Level3Input Parse(InputReader reader)
    {
        var count = reader.Int();
        var cases = new Level3Case[count];
        for (var i = 0; i < count; i++)
        {
            reader.SkipBlank();
            var h = reader.Ints(); // radius rows columns
            var c = new Level3Case(h[0], h[1], h[2]);
            for (var r = 0; r < c.Rows; r++)
                c.Grid[r] = reader.Line();
            cases[i] = c;
        }
        return new Level3Input(cases);
    }

    public override IEnumerable<string> Solve(Level3Input input) =>
        input.Cases.Select(SolveCase);


    private static string SolveCase(Level3Case c)
    {
        var result = PrintNano(c);
        return result;
    }

    private static string PrintNano(Level3Case level3Case)
    {
        var emitters = CollectEmitters(level3Case);
        foreach (var e in emitters) ApplyEmitter(level3Case, e);
        return string.Join("\n", level3Case.Grid);
    }

    // All digits in the grid, in reading order; Row/Column are 0-based indices into Grid[row][column].
    private static IEnumerable<Level3Emitter> CollectEmitters(Level3Case c)
    {
        for (var row = 0; row < c.Rows; row++)
        {
            var line = c.Grid[row];
            for (var column = 0; column < line.Length; column++)
            {
                if (char.IsDigit(line[column]))
                    yield return new Level3Emitter(row, column, line[column] - '0');
            }
        }
    }

    // Emitter number → (dRow, dCol) step; taken from the example output.
    private static (int DRow, int DCol) Direction(int number) => number switch
    {
        1 => (0, 2),
        2 => (1, 1),
        3 => (1, -1),
        4 => (0, -2),
        5 => (-1, -1),
        6 => (-1, 1),
        _ => throw new ArgumentOutOfRangeException(nameof(number), number, "Unknown emitter number"),
    };

    // From the emitter, step in its direction and turn O into M until the next cell is not O.
    private static void ApplyEmitter(Level3Case c, Level3Emitter e)
    {
        var (dRow, dCol) = Direction(e.Number);
        var row = e.Row + dRow;
        var column = e.Column + dCol;
        while (row >= 0 && row < c.Rows && column >= 0 && column < c.Grid[row].Length && c.Grid[row][column] == 'O')
        {
            var chars = c.Grid[row].ToCharArray();
            chars[column] = 'M';
            c.Grid[row] = new string(chars);
            row += dRow;
            column += dCol;
        }
    }
}
