# AGENTS.md

This file provides guidance to AI agents when working with code in this repository.

## Documentation Structure

**IMPORTANT: This file provides high-level guidance. For detailed development workflows, read:**
- **@DEVELOPMENT.md** - Comprehensive development guide (template structure, workflows, testing, troubleshooting)

**For AI Agents:** When working on template modifications, you MUST read @DEVELOPMENT.md first using the Read tool to understand:
- Complete shared files system and synchronization rules
- Step-by-step modification workflows for different scenarios
- Template engine details and conditional directives
- Testing strategies and troubleshooting guides

## Project Overview

This is a .NET template package (Inflop.ConsoleApp.Templates) containing 4 console application templates with varying complexity levels. The templates are distributed as a NuGet package that users install via `dotnet new install`.

## Minimum Requirements

- **.NET SDK 8.0 or later** - Templates support .NET 8.0 (LTS), .NET 9.0 (STS), and .NET 10.0 (LTS, latest)
- **NuGet Package Versions** - All packages use range syntax `[8.0.0,)` for automatic updates within compatibility
- **End-of-Life Versions** - .NET 6.0 and 7.0 are no longer supported

## Architecture

**Template Package Structure:**
- `Inflop.ConsoleApp.Templates.csproj` - The main template package project (netstandard2.0, PackageType=Template)
- `templates/` directory contains 4 separate template projects:
  1. `1-ConsoleApp.Simple/` - Manual DI container setup (no Generic Host)
  2. `2-ConsoleApp.Standard/` - Generic Host pattern with BackgroundService
  3. `3-ConsoleApp.Advanced/` - Top-level statements with extension methods pattern
  4. `4-ConsoleApp.Enterprise/` - Strongly-typed configuration with Options pattern

Each template has a `.template.config/template.json` file that defines template metadata, short names, and parameters (like target framework selection).

**Key Architectural Patterns:**
- Template 1 (Simple) uses manual `ServiceCollection` setup with **inline service registration** for maximum simplicity
- Templates 2, 3, 4 use Generic Host pattern (`Host.CreateApplicationBuilder`)
- Template 3 (Advanced) uses extension methods for service registration (`ServiceExtensions.cs`)
- Template 4 (Enterprise) uses centralized configuration class (`ServiceConfiguration.cs`)
- Background workers implement `BackgroundService` base class (Templates 2, 3, 4)
- Enterprise template demonstrates Options pattern with strongly-typed configuration classes

**Async/Await Configuration:**
- All templates support both async and synchronous patterns via `--use-async` parameter
- **Template 1 (Simple)**: Default `false` (synchronous) - suitable for beginners
- **Templates 2, 3, 4**: Default `true` (asynchronous) - production-ready patterns
- Synchronous versions use `.GetAwaiter().GetResult()` with warning comments where necessary
- Async/sync affects: Main/Worker execution, Database operations, HTTP Client calls, Messaging operations

## Common Commands

**Building the template package:**
```bash
# IMPORTANT: Use --no-build to avoid compiling template code with conditional directives
dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release --no-build
```
The output .nupkg file will be in `src/bin/Release/`.

**Why --no-build?** Template files contain conditional compilation directives (`//#if`, `//#endif`) that are processed by the template engine, not the C# compiler. Building template projects will cause compilation errors.

**Testing templates locally:**
```bash
# Install from local .nupkg
dotnet new install src/bin/Release/Inflop.ConsoleApp.Templates.1.0.0.nupkg

# Create test project from a template
dotnet new inflop-simple -n TestApp
dotnet new inflop-standard -n TestApp2
dotnet new inflop-advanced -n TestApp3
dotnet new inflop-enterprise -n TestApp4

# Test with different target frameworks (net8.0, net9.0, net10.0 supported)
dotnet new inflop-simple -n TestNet8 -F net8.0
dotnet new inflop-simple -n TestNet9 -F net9.0
dotnet new inflop-simple -n TestNet10 -F net10.0

# Test with async/await parameter
dotnet new inflop-simple -n TestSimpleAsync --use-async true
dotnet new inflop-standard -n TestStandardSync --use-async false

# Run a test project
cd TestApp
dotnet run
```

**Uninstalling the template during development:**
```bash
dotnet new uninstall Inflop.ConsoleApp.Templates
```

**Building the solution:**
```bash
cd src
dotnet build Inflop.ConsoleApp.Templates.sln
```

**Running individual template projects for testing:**
```bash
# Navigate to specific template
cd src/templates/1-ConsoleApp.Simple
dotnet run

# Run with environment
dotnet run --environment Development
dotnet run --environment Production
```

## Template Configuration

Each template's behavior is controlled by `template.json`:
- `shortName`: The identifier used with `dotnet new` (e.g., "inflop-simple")
- `sourceName`: The namespace placeholder that gets replaced (e.g., "ConsoleApp.Simple")
- `symbols`: Parameters like `Framework` that allow users to choose .NET version
- **Supported frameworks:** net8.0 (LTS, default), net9.0 (STS), net10.0 (LTS, latest)
- **Deprecated:** net6.0 and net7.0 (removed due to end-of-life)

## Project Structure Notes

**Template Package Project (`Inflop.ConsoleApp.Templates.csproj`):**
- Uses `TargetFramework=netstandard2.0` (required for template packages)
- Has `IncludeBuildOutput=false` and `PackageType=Template`
- Explicitly excludes template .cs files from compilation
- Includes all template files via `<Content Include="templates/**/*">`
- Excludes bin/obj directories from the package

**Individual Templates:**
- All target net8.0 (LTS) by default, with net9.0 and net10.0 available via template parameter
- **Package versions:** Use range syntax `[8.0.0,)` to ensure minimum version with automatic updates
- Configuration follows priority: appsettings.json → appsettings.{Environment}.json → Environment variables → Command-line args
- Core dependencies use Microsoft.Extensions.* 8.0+ for compatibility

## Development Workflow

**⚠️ For detailed step-by-step workflows, see @DEVELOPMENT.md**

Before modifying templates, read @DEVELOPMENT.md which contains:
- Complete shared files list and synchronization rules
- Step-by-step guides for modifying shared files, template-specific files, adding new templates, and adding features
- Template engine details (conditional directives)
- Testing strategies and troubleshooting

**Quick Reference:**

**Shared Files** (edit in `src/_shared/` → run `./src/sync-shared-files.sh`):
- `.template.config/dotnetcli.host.json`
- `Extensions/*.cs` (except Simple template)
- `Infrastructure/*.cs`
- `Services/IApiClient.cs` and `Services/ApiClient.cs`
- `Messaging/*.cs`, `Health/*.cs`, `Data/*.cs`

**Template-Specific Files** (edit directly in template directory):
- `Program.cs`, `Workers/MyWorker.cs`, `Services/MyService.cs`
- `ServiceConfiguration.cs` (Enterprise only)
- `.template.config/template.json`

**Critical Rules:**
- ✅ Shared file → Edit in `src/_shared/` → Run sync script
- ✅ Template-specific file → Edit directly in template directory
- ❌ **NEVER edit** synchronized files directly in template directories - changes will be overwritten!
- ❌ **NEVER build** template projects directly - use `dotnet pack --no-build`

**Basic workflow:**
1. Check if file is shared (see list above or @DEVELOPMENT.md)
2. Edit in appropriate location (`_shared/` or template directory)
3. Run `./src/sync-shared-files.sh` (if shared file modified)
4. Pack: `dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release --no-build`
5. Test: Uninstall → Install → Generate → Build → Run

For detailed workflows, troubleshooting, and best practices, see @DEVELOPMENT.md
