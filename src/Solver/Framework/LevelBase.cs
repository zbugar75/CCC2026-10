namespace Solver.Framework;

public interface ILevel
{
    int Number { get; }

    IEnumerable<string> Run(InputReader reader);
}

/// <summary>
/// A contest level: parse the input file into <typeparamref name="TInput"/>, then solve it into output lines.
/// </summary>
public abstract class LevelBase<TInput> : ILevel
{
    public abstract int Number { get; }

    /// <summary>Input file → structure.</summary>
    public abstract TInput Parse(InputReader reader);

    /// <summary>Structure → output lines.</summary>
    public abstract IEnumerable<string> Solve(TInput input);

    public IEnumerable<string> Run(InputReader reader) => Solve(Parse(reader));
}
