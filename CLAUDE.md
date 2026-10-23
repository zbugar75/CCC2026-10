# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

Cloudflight Coding Contest (CCC) solution skeleton. `src/Solver` targets **net8.0**,
`tests/Solver.Tests` targets net10.0 (needs the .NET 10 SDK). Contest-speed code: during the
contest only `src/Solver/Levels/LevelN.cs` is edited.

## Commands

The root holds both `CCC202610.sln` and `CCC202610.slnx`, so bare `dotnet build`/`dotnet test`
fail with MSB1011 — always name the solution:

    dotnet build CCC202610.slnx
    dotnet run --project src/Solver                                                  # run the current level on all its inputs
    dotnet test --solution CCC202610.slnx                                            # example check for every level
    dotnet test --solution CCC202610.slnx --filter-class "Solver.Tests.Level3Tests"  # example check for one level

`global.json` opts `dotnet test` into Microsoft.Testing.Platform (required for xUnit v3 on the .NET 10 SDK),
which is why it takes `--solution`/`--filter-class` rather than the VSTest-style positional path/`--filter`.

## Workflow per level

1. Unzip the level's files into `Input/LevelN/` (`levelN_example.in`, `levelN_example.out`, `levelN_1..5.in`, PDF).
2. In `Levels/LevelN.cs`: replace the `LevelNInput` record with the level's structure, adapt `Parse`
   (default: first line = case count, one case per line), implement `Solve` (returns output lines).
3. `dotnet test --filter-class "Solver.Tests.LevelNTests"` until the example matches.
4. Switch the hardcoded level in `src/Solver/Program.cs` (`ILevel level = new LevelN();`) and run;
   results land in `Output/LevelN/<input name>.out` for upload.

The level is intentionally hardcoded in `Program.cs`, not a command-line argument.

## Structure

- `Framework/InputReader.cs` — line cursor: `Line`, `Lines(n)`, `Rest`, `Peek`, `Int`, `Long`,
  `Ints`, `Longs`, `Tokens` (split on space/tab/comma/semicolon), `Grid(rows)`.
- `Framework/LevelBase.cs` — `LevelBase<TInput>`: `Parse(InputReader) → TInput`, `Solve(TInput) → IEnumerable<string>`.
  The level number (`Number`) drives the Input/Output folder names.
- `Framework/Runner.cs` — `Run` processes every `*.in` of the level (example first), writes outputs,
  reports per-file errors without stopping; `Execute` returns one file's output text (used by tests).
- `Framework/Paths.cs` — finds the repo root by walking up to the folder containing `Input/`;
  the example is any `*example*.in` with a sibling `.out`.
- `tests/Solver.Tests/LevelTestBase.cs` — runs the example input through the level, writes our
  result to `Output/LevelN/`, and compares with the provided `.out` (line endings and trailing
  whitespace normalized). Skipped while the level has no example pair. `LevelNTests` are one-line subclasses.

Levels 1–7 are pre-created; for an 8th level copy `Level7.cs`/`Level7Tests.cs` and create `Input/Level8`.
