using AwesomeGame2.Shared.ViewModels;

namespace AwesomeGame2.Shared.Results;

public sealed record GameCommandResult(
    ScreenViewModel Screen,
    IReadOnlyList<string> Messages,
    bool SaveChanged,
    string? Error);
