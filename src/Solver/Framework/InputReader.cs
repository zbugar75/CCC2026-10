namespace Solver.Framework;

/// <summary>
/// Line-based cursor over an input file. Every read consumes the next line.
/// </summary>
public sealed class InputReader
{
    private static readonly char[] Separators = [' ', '\t', ',', ';'];

    private readonly string[] _lines;
    private int _position;

    public InputReader(IEnumerable<string> lines) => _lines = lines.ToArray();

    public static InputReader FromFile(string path) => new(File.ReadAllLines(path));

    public bool HasMore => _position < _lines.Length;

    public int LineNumber => _position + 1;

    /// <summary>Next line without consuming it.</summary>
    public string Peek() => HasMore ? _lines[_position] : throw EndOfInput();

    /// <summary>Next raw line.</summary>
    public string Line() => HasMore ? _lines[_position++] : throw EndOfInput();

    /// <summary>Next <paramref name="count"/> raw lines.</summary>
    public string[] Lines(int count) => Enumerable.Range(0, count).Select(_ => Line()).ToArray();

    /// <summary>All remaining lines.</summary>
    public string[] Rest() => Lines(_lines.Length - _position);

    /// <summary>Next line split into tokens (space, tab, comma, semicolon).</summary>
    public string[] Tokens() => Line().Split(Separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    /// <summary>Next line parsed as a single int.</summary>
    public int Int() => int.Parse(Line().Trim());

    /// <summary>Next line parsed as a single long.</summary>
    public long Long() => long.Parse(Line().Trim());

    /// <summary>Next line parsed as a list of ints.</summary>
    public int[] Ints() => Tokens().Select(int.Parse).ToArray();

    /// <summary>Next line parsed as a list of longs.</summary>
    public long[] Longs() => Tokens().Select(long.Parse).ToArray();

    /// <summary>Next <paramref name="rows"/> lines as a char grid, indexed [row][column].</summary>
    public char[][] Grid(int rows) => Lines(rows).Select(l => l.ToCharArray()).ToArray();

    private InvalidOperationException EndOfInput() =>
        new($"Unexpected end of input after line {_lines.Length}.");
}
