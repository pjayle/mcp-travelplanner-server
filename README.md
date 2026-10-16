# TravelMcp.Server

Server-side solution for Travel Planner (TravelMcp.Server). This repository contains the .NET 9 backend services and APIs.

## Contents
- Solution: TravelMcp.Server.slnx
- Typical entry project(s): (see solution by opening the .slnx file)
- Target framework: .NET 9

## Prerequisites
- .NET SDK 9.x (install from https://dotnet.microsoft.com)
- Visual Studio 2022/2026 or VS Code (with C# extension)
- Git
- Optional: Docker (if you plan to run containerized builds)

## Setup (local)
1. Clone:
   git clone https://github.com/<your-org>/mcp-travelplanner-server.git
   cd TravelMcp.Server

2. Open solution:
   - Visual Studio: open `TravelMcp.Server.slnx`
   - CLI: dotnet restore

3. Configure environment:
   - Copy any example configuration files (e.g. `appsettings.Development.json.example -> appsettings.Development.json`) and update secrets/connection strings.
   - Environment variables commonly required:
     - ConnectionStrings__DefaultConnection
     - ASPNETCORE_ENVIRONMENT (Development/Production)
     - Any API keys used by the project (map to your secrets store)
   - For CI/CD, use GitHub Secrets to store sensitive values.

## Build
From repository root:
dotnet build TravelMcp.Server.slnx -c Release

## Run (development)
dotnet run --project <project-path> -c Debug
Example (if your API project is at src/Api):
dotnet run --project src/Api/Api.csproj

Or use Visual Studio to launch with IIS Express or the configured launch profile.

## Tests
List and run tests:
dotnet test TravelMcp.Server.slnx --no-build -v minimal

## Docker (optional)
Add a Dockerfile to project(s) you want to containerize. Example build/publish:
docker build -t travelmcp/server:latest .
docker run -e ASPNETCORE_ENVIRONMENT=Production -p 5000:80 travelmcp/server:latest

## CI (GitHub Actions)
A sample workflow is provided in `.github/workflows/dotnet.yml` (see below). Configure repository secrets for any required connection strings or deployment credentials.

## Database / Migrations
If the solution uses EF Core or migrations:
- To create migrations:
  dotnet ef migrations add <Name> --project <DbProject> --startup-project <StartupProject>
- To apply migrations:
  dotnet ef database update --project <DbProject> --startup-project <StartupProject>

## Common Tasks
- Restore: dotnet restore
- Clean: dotnet clean
- Pack (nuget): dotnet pack

## Contributing
- Use feature branches and open PRs against `master` (or the designated main branch).
- Add tests for new features and fix failing tests before opening PRs.
- Follow existing code style and run `dotnet format` if needed.

## GitHub repository configuration suggestions
- Protect `master` branch: require PR reviews and passing CI.
- Add required status checks for build & test workflow.
- Store secrets (DB connection strings, API keys) in GitHub Secrets.
- Optionally add Dependabot for dependency updates.

## License
Specify your license (e.g., MIT). Add `LICENSE` file to the repo.

## Contact
Project maintainer or repo owner (add appropriate details).
