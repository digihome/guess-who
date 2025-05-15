using ClientService;
using DigiHome.GuessWho.Library;

var result = AppTypeDetector.Detect();

Console.WriteLine($"Detected application type: {result.Display}");

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
