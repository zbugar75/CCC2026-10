using Solver.Framework;

namespace Solver.Levels;

public record Level4Input(Level4Case[] Cases);

public readonly struct Level4Case(int radius, int rows, int columns, Level4ProgramLine[] program)
{
    public int Radius { get; } = radius;
    public int Rows { get; } = rows;

    public int Columns { get; } = columns;

    public Level4ProgramLine[] Program { get; } = program;

    public string[] Grid { get; } = new string[rows];

    public int[,] Hits { get; } = new int[rows, columns];
}

public enum EmitterStatus { Off, On }

public readonly record struct Level4Emitter(int Row, int Column, int Number, EmitterStatus Status = EmitterStatus.Off);

// One program line: 6 instructions (F, B, T or -), index 0..5 = emitter 1..6.
public readonly record struct Level4ProgramLine(char[] Instructions)
{
    public char For(int emitterNumber) => Instructions[emitterNumber - 1];
}

public sealed class Level4 : LevelBase<Level4Input>
{
    public override int Number => 4;

    // First line = number of cases; each case = radius, number of program lines, then the program lines
    // (6 instructions F/B/T/- per line, one per emitter); cases are separated by blank lines.
    public override Level4Input Parse(InputReader reader)
    {
        var count = reader.Int();
        var cases = new Level4Case[count];
        for (var i = 0; i < count; i++)
        {
            reader.SkipBlank();
            var radius = reader.Int();
            var lineCount = reader.Int();
            var program = new Level4ProgramLine[lineCount];
            for (var l = 0; l < lineCount; l++)
                program[l] = ParseProgramLine(reader);
            var c = new Level4Case(radius, 2 * radius + 1, 4 * radius + 1, program);
            Level1.GenerateGrid(radius).CopyTo(c.Grid, 0); // empty board from Level 1
            cases[i] = c;
        }
        return new Level4Input(cases);
    }

    private static Level4ProgramLine ParseProgramLine(InputReader reader)
    {
        var lineNumber = reader.LineNumber;
        var tokens = reader.Tokens();
        if (tokens.Length != 6 || tokens.Any(t => t is not ("F" or "B" or "T" or "-")))
            throw new FormatException($"Line {lineNumber}: expected 6 instructions of F/B/T/-, got '{string.Join(' ', tokens)}'.");
        return new Level4ProgramLine(tokens.Select(t => t[0]).ToArray());
    }

    public override IEnumerable<string> Solve(Level4Input input) =>
        input.Cases.Select(SolveCase);


    private static string SolveCase(Level4Case c)
    {
        var result = PrintNano(c);
        return result;
    }

    private static string PrintNano(Level4Case level4Case)
    {
        var level4Emitters = FixedEmitters(level4Case).ToArray();
        foreach (var p in level4Case.Program)
        {
            ClearHits(level4Case);
            ApplyProgramOnEmitter(level4Case, level4Emitters, p);
            foreach (var e in level4Emitters)
                if (e.Status == EmitterStatus.On)
                    ApplyEmitter(level4Case, e);

            ApplyHits(level4Case);
        }

        return string.Join("\n", level4Case.Grid);
    }

    // Applies one program line: instruction i belongs to emitter i+1.
    // F = one cell clockwise, B = one cell counterclockwise, T = toggle On/Off, - = no operation.
    private static void ApplyProgramOnEmitter(Level4Case level4Case, Level4Emitter[] level4Emitters, Level4ProgramLine level4ProgramLine)
    {
        for (var i = 0; i < level4Emitters.Length; i++)
        {
            var e = level4Emitters[i];
            level4Emitters[i] = level4ProgramLine.For(e.Number) switch
            {
                'F' => MoveTo(e, Clockwise(e.Column + 1, e.Row + 1, level4Case.Radius)),
                'B' => MoveTo(e, CounterClockwise(e.Column + 1, e.Row + 1, level4Case.Radius)),
                'T' => e with { Status = e.Status == EmitterStatus.On ? EmitterStatus.Off : EmitterStatus.On },
                '-' => e,
                var op => throw new InvalidOperationException($"Unknown instruction '{op}'."),
            };
        }
    }

    private static Level4Emitter MoveTo(Level4Emitter e, (int X, int Y) p) => e with { Row = p.Y - 1, Column = p.X - 1 };

    // One cell clockwise along the border (1-based X = column, Y = row).
    // The last rule (upper-left edge) is not in the given list but needed, e.g. for the left corner.
    private static (int X, int Y) Clockwise(int x, int y, int rad)
    {
        if (y == 1 && x < 3 * rad + 1) return (x + 2, y);                 // top edge →
        if (y < rad + 1 && x > 3 * rad) return (x + 1, y + 1);            // upper-right edge ↘
        if (y > rad && x > 3 * rad + 1) return (x - 1, y + 1);            // lower-right edge ↙
        if (y == 2 * rad + 1 && x > rad + 1) return (x - 2, y);           // bottom edge ←
        if (y > rad + 1 && x <= rad + 1) return (x - 1, y - 1);           // lower-left edge ↖
        if (y <= rad + 1 && x < rad + 1) return (x + 1, y - 1);           // upper-left edge ↗
        throw new InvalidOperationException($"({x},{y}) is not a border cell for radius {rad}.");
    }

    // One cell counterclockwise along the border: the inverse of Clockwise.
    private static (int X, int Y) CounterClockwise(int x, int y, int rad)
    {
        if (y == 1 && x > rad + 1) return (x - 2, y);                     // top edge ←
        if (y <= rad + 1 && x > 3 * rad + 1) return (x - 1, y - 1);       // upper-right edge ↖
        if (y > rad + 1 && x >= 3 * rad + 1) return (x + 1, y - 1);       // lower-right edge ↗
        if (y == 2 * rad + 1 && x < 3 * rad + 1) return (x + 2, y);       // bottom edge →
        if (y >= rad + 1 && x < rad + 1) return (x + 1, y + 1);           // lower-left edge ↘
        if (y < rad + 1 && x <= rad + 1) return (x - 1, y + 1);           // upper-left edge ↙
        throw new InvalidOperationException($"({x},{y}) is not a border cell for radius {rad}.");
    }

    // Resets every hit counter to 0 for the next step.
    private static void ClearHits(Level4Case level4Case) => Array.Clear(level4Case.Hits);

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
