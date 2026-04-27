using System;

namespace AwesomeGame2.Tests
{
    public static class Program
    {
        public static int Main()
        {
            try
            {
                GameArchitectureTests.RunAll();
                Console.WriteLine("All architecture tests passed.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Architecture test failure: " + ex.Message);
                return 1;
            }
        }
    }
}
