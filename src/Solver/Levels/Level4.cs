using Solver.Framework;

namespace Solver.Levels;

public record Level4Input(Level4Case[] Cases);

public readonly struct Level4Case(int radius, int rows, int columns)
{
    public int Radius { get; } = radius;
    public int Rows { get; } = rows;

    public int Columns { get; } = columns;

    public string[] Grid { get; } = new string[rows];

    public int[,] Hits { get; } = new int[rows, columns];
}

public enum EmitterStatus { Off, On }

public readonly record struct Level4Emitter(int Row, int Column, int Number, EmitterStatus Status = EmitterStatus.Off);

public sealed class Level4 : LevelBase<Level4Input>
{
    public override int Number => 4;

    // No board is loaded; the input loading follows in a later step.
    public override Level4Input Parse(InputReader reader) =>
        throw new NotImplementedException("Level 4 input loading: follow-up");

    public override IEnumerable<string> Solve(Level4Input input) =>
        input.Cases.Select(SolveCase);


    private static string SolveCase(Level4Case c)
    {
        var result = PrintNano(c);
        return result;
    }

    private static string PrintNano(Level4Case level4Case)
    {
        foreach (var e in FixedEmitters(level4Case)) ApplyEmitter(level4Case, e);
        ApplyHits(level4Case);
        return string.Join("\n", level4Case.Grid);
    }

    // 0–1 hits: unchanged; exactly 2 hits: M (matter); 3+ hits: O (destroyed).
    private static void ApplyHits(Level4Case level4Case)
    {
        for (var row = 0; row < level4Case.Rows; row++)
        {
            var chars = level4Case.Grid[row].ToCharArray();
            for (var column = 0; column < level4Case.Columns; column++)
            {
                var hits = level4Case.Hits[row, column];
                if (hits < 2) continue;
                if (chars[column] is not ('O' or 'M'))
                    throw new InvalidOperationException(
                        $"Cell ({row},{column}) has {hits} hits but holds '{chars[column]}' instead of O/M.");
                chars[column] = hits == 2 ? 'M' : 'O';
            }
            level4Case.Grid[row] = new string(chars);
        }
    }

    // The 6 emitters sit on the hexagon corners; positions given 1-based (X = column, Y = row).
    // Emitter 4 is the right corner X=4*rad+1, Y=rad+1 (mirror of emitter 1).
    private static IEnumerable<Level4Emitter> FixedEmitters(Level4Case c)
    {
        var rad = c.Radius;
        (int X, int Y)[] positions =
        [
            (1, rad + 1),               // 1
            (rad + 1, 1),               // 2
            (3 * rad + 1, 1),           // 3
            (4 * rad + 1, rad + 1),     // 4
            (3 * rad + 1, 2 * rad + 1), // 5
            (rad + 1, 2 * rad + 1),     // 6
        ];
        for (var i = 0; i < positions.Length; i++)
            yield return new Level4Emitter(positions[i].Y - 1, positions[i].X - 1, i + 1, EmitterStatus.Off);
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

    // From the emitter, step in its direction over O/M cells and count the hits.
    // Only counts hits; the grid itself is updated later by ApplyHits.
    private static void ApplyEmitter(Level4Case c, Level4Emitter e)
    {
        var (dRow, dCol) = Direction(e.Number);
        var row = e.Row + dRow;
        var column = e.Column + dCol;
        while (row >= 0 && row < c.Rows && column >= 0 && column < c.Columns && c.Grid[row][column] is 'O' or 'M')
        {
            c.Hits[row, column]++;
            row += dRow;
            column += dCol;
        }
    }
}
