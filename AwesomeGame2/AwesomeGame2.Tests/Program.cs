using AwesomeGame2.Tests;

try
{
    GameArchitectureTests.RunAll();
    Console.WriteLine("All architecture tests passed.");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Architecture test failure: {ex.Message}");
    return 1;
}
