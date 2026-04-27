namespace AwesomeGame2.Shared.Commands;

public sealed record StartNewGameCommand(string PlayerName) : GameCommand;
