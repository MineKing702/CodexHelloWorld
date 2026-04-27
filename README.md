# AwesomeGame2 (Stage 3)

AwesomeGame2 is split into a **Unity-ready shared gameplay library** and a **console client**.

## Project structure

- `AwesomeGame2/AwesomeGame2.Shared`
  - Pure gameplay logic, commands, save serialization, validation, content definitions, and screen viewmodels.
  - No Console API usage.
  - No Unity API usage.
- `AwesomeGame2/AwesomeGame2Console`
  - Console renderer and input loop.
  - Dispatches commands to shared managers.
  - Performs file path selection for save/load.
- `AwesomeGame2/AwesomeGame2.Tests`
  - Regression test harness for architecture and save/load behavior.

## Build

```bash
dotnet build AwesomeGame2/AwesomeGame2.slnx
```

## Test

```bash
dotnet test AwesomeGame2/AwesomeGame2.slnx
dotnet run --project AwesomeGame2/AwesomeGame2.Tests
```

## Run console

```bash
dotnet run --project AwesomeGame2/AwesomeGame2Console
```

In the console, use menu numbers or command IDs. Special commands:

- `:save <path>`
- `:load <path>`
- `:quit`

## Architecture rule: `GameSave` is the single source of truth

All mutable gameplay state is rooted in `GameSave` and its owned model graph.
Managers mutate only `GameSave` and return read-only viewmodels/results.

## Unity integration guidance

Future Unity UI should:

1. Keep one active `GameSave` instance.
2. Create and dispatch shared-library `GameCommand` objects (for UI buttons, use `MenuCommand`).
3. Render the returned `ScreenViewModel` and any toast/error messages.
4. Call shared serializer APIs for save/load JSON and files (or map to Unity storage APIs while still using shared JSON methods).

This keeps gameplay deterministic and portable across console and Unity clients.
