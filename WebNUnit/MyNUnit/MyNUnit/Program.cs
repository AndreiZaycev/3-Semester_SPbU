namespace MyNUnit;

internal static class Program
{
    private static void Main(string[] args)
    {
        try
        {
            Runner.Run("C:\\Users\\Андрей\\Desktop\\4Sem\\WebNUnit\\WebNUnit\\Tests");
            Console.Write("aue");
        }
        catch (Exception ex)
        {
            Console.Write("beb");
        }

        try
        {
            Console.WriteLine("Start testing...");
            Runner.Run(args[0]);
            foreach (var test in Runner.TestInformation!)
            {
                var message = test.Result switch
                {
                    "Errored" => $"{test.Name} errored with the message: {test.ErrorMessage}",
                    "Ignored" => $"{test.Name} ignored with the ignore reason: {test.IgnoreReason}",
                    "Passed" => $"{test.Name} passed in {test.Time} seconds",
                    "Failed" => $"{test.Name} failed",
                    _ => throw new ArgumentOutOfRangeException()
                };
                Console.WriteLine(message);
            }

            Console.WriteLine("Testing is over");
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("Error: Directory not found");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }
}