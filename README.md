# GuessWho – CLI and Library for Detecting .NET Application Types

[![NuGet CLI](https://img.shields.io/nuget/v/DH.GuessWho.Tool.svg?label=GuessWho.Tool&logo=nuget)](https://www.nuget.org/packages/DH.GuessWho.Tool)
[![NuGet Library](https://img.shields.io/nuget/v/DH.GuessWho.Library.svg?label=GuessWho.Library&logo=nuget)](https://www.nuget.org/packages/DH.GuessWho.Library)
![License](https://img.shields.io/github/license/digihome/guess-who)

**GuessWho** is a command-line tool (CLI) and .NET class library that allows you to determine what kind of application a given .NET assembly is — for example, whether it's a console app, ASP.NET app, Windows Forms, WPF, Worker Service, and more.

The tool analyzes the metadata and dependencies of a `.dll` or `.exe` to identify the runtime context (e.g., web app, desktop app, Windows service, etc.). This can be useful when writing libraries that need to adjust their behavior depending on the host application type.

The `GuessWho.App` is packaged as a global .NET CLI tool, and `GuessWho.Library` is a reusable class library for integration into your own .NET code.

## Installation

- **CLI**:
  ```bash
  dotnet tool install -g DH.GuessWho.Tool
  ```

- **Library**:
  ```bash
  dotnet add package DH.GuessWho.Library
  ```

## Usage (CLI)

```bash
dotnet guesswho <path_to_assembly> [options]
```

### Options:
- `-f`, `--full` – Show referenced assemblies
- `-n`, `--no-logo` – Suppress the banner
- `--version` – Display version information
- `--help` – Show usage help

Example:
```bash
dotnet guesswho .\src\Example\ClientService\bin\net8.0\ClientService.dll
 _____                     _    _ _
|  __ \                   | |  | | |
| |  \/_   _  ___  ___ ___| |  | | |__   ___
| | __| | | |/ _ \/ __/ __| |/\| | '_ \ / _ \
| |_\ \ |_| |  __/\__ \__ \  /\  / | | | (_) |
\____ /\__,_|\___||___/___/\/  \/|_| |_|\___/

🕵️ Guess Who...
📁 Assembly: \src\Example\ClientService\bin\net8.0\ClientService.dll
🔍 Detected: Client Console, Service
⚙️ TFM : .NETCoreApp,Version=v8.0
```

## Usage in Code

```csharp
using GuessWho.Library;

var result = AppTypeDetector.Detect();

Console.WriteLine($"Detected application type: {result.Display}");
```

## Supported Application Types

GuessWho currently detects:

- ✅ Console applications
- ✅ Windows Forms (WinForms)
- ✅ ASP.NET Core (Web API, MVC)
- ✅ ASP.NET (Full Framework)
- ✅ Worker Service (.NET background services)
- ⏳ Windows Service
- ✅ WCF services
- ✅ SOAP services
- ⏳ WPF (Windows Presentation Foundation) - in progress

## Examples

The [`Example`](https://github.com/digihome/guess-who/tree/main/Example) directory in this repository includes working projects for each supported application type. You can build and inspect them with GuessWho to see how detection works in practice.

Examples include:

- Console App (Framework, Core)
- WinForms (Framework, Core)
- ASP.NET (Framework, Core)
- Worker Service (Framework, Core)
- SOAP/ASMX (Framework)
- WCF Service
- WPF (Framework, Core)

## Components

- **GuessWho.App** – CLI tool (install via `dotnet tool`)
- **GuessWho.Library** – .NET class library (install via NuGet)

