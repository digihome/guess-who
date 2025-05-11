using GuessWho.Library;

var result = AppTypeDetector.Detect();

Console.WriteLine($"Detected application type: {result.Display}");
