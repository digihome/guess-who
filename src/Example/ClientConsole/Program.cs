using DigiHome.GuessWho.Library;

internal class Program
{
    private static void Main(string[] args)
    {
        var result = AppTypeDetector.Detect();

        Console.WriteLine($"Detected application type: {result.Display}");

    }
}