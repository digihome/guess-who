using ClientService;
using DigiHome.GuessWho.Library;
using System.Threading.Tasks.Sources;

var result = AppTypeDetector.Detect();

Console.WriteLine($"Detected application type: {result.Display}");

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

App.Init(host);
var x = App.Services.GetRequiredService<Worker>();
host.Run();
