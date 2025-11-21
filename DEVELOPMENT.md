# Development Guide

This guide provides detailed information for developers working on the Inflop.ConsoleApp.Templates project.

## Table of Contents

1. [Quick Start for Developers](#quick-start-for-developers)
2. [Template Structure Deep Dive](#template-structure-deep-dive)
3. [Shared Files System](#shared-files-system)
4. [Development Workflows](#development-workflows)
5. [Template Engine & Conditional Directives](#template-engine--conditional-directives)
6. [Building & Packaging](#building--packaging)
7. [Publishing to NuGet](#publishing-to-nuget)
8. [Testing Strategy](#testing-strategy)
9. [Common Issues & Troubleshooting](#common-issues--troubleshooting)
10. [Best Practices](#best-practices)

---

## Quick Start for Developers

**Prerequisites:**
- .NET SDK 8.0 or later
- Basic understanding of .NET templates
- Bash shell (for sync script)

**First-time setup:**
```bash
# Clone the repository
git clone <repository-url>
cd Inflop.ConsoleApp.Templates

# Explore the structure
ls -la src/templates/
ls -la src/_shared/
```

**Typical development cycle:**
1. Make changes to shared or template-specific files
2. Run `./scripts/sync-shared-files.sh` (if you modified shared files)
3. Pack the template: `dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release`
4. Test locally by installing and creating test projects
5. Verify generated projects compile and run correctly

**Or use automated testing:**
```bash
./scripts/test-all-templates.sh        # Comprehensive test suite (64 tests)
./scripts/test-single-template.sh inflop-simple MyTest --add-serilog
```

---

## Template Structure Deep Dive

### Project Root Structure

```
Inflop.ConsoleApp.Templates/
├── scripts/                                 # Shell scripts directory
│   ├── sync-shared-files.sh                 # Synchronization script
│   ├── test-all-templates.sh                # Comprehensive test suite (64 tests)
│   ├── test-single-template.sh              # Single template testing
│   ├── show-template-help.sh                # Show template parameters
│   ├── debug-test.sh                        # Debug testing script
│   ├── TEST-PLAN.md                         # Comprehensive test plan documentation
│   └── TESTING-README.md                    # Quick testing guide
├── src/
│   ├── Inflop.ConsoleApp.Templates.csproj   # Main template package project
│   ├── Inflop.ConsoleApp.Templates.sln      # Solution file
│   ├── _shared/                             # Shared files source
│   │   ├── .template.config/
│   │   ├── Extensions/
│   │   ├── Infrastructure/
│   │   ├── Services/
│   │   ├── Messaging/
│   │   ├── Health/
│   │   └── Data/
│   └── templates/                           # Individual template projects
│       ├── 1-ConsoleApp.Simple/
│       ├── 2-ConsoleApp.Standard/
│       ├── 3-ConsoleApp.Advanced/
│       └── 4-ConsoleApp.Enterprise/
├── CLAUDE.md                                # AI assistant instructions
├── AGENTS.md                                # AI agent instructions
├── DEVELOPMENT.md                           # This file
└── README.md                                # End-user documentation
```

### Template Package Project (`Inflop.ConsoleApp.Templates.csproj`)

This is a **special type of project** that acts as a container for .NET templates:

**Key properties:**
```xml
<TargetFramework>netstandard2.0</TargetFramework>
<PackageType>Template</PackageType>
<IncludeBuildOutput>false</IncludeBuildOutput>
```

**Why netstandard2.0?**
- Required for template packages by .NET templating engine
- The templates themselves can target different frameworks (net8.0, net9.0, net10.0)

**Why IncludeBuildOutput=false?**
- Template packages don't include compiled assemblies
- They include source code files that will be processed by the template engine

**Content inclusion:**
```xml
<Content Include="templates/**/*" Exclude="**/bin/**;**/obj/**" />
```
- Includes ALL files from templates/ directory
- Excludes build artifacts (bin/, obj/)

### Individual Template Projects

Each template is a **complete, functional .NET console application** with its own:

#### 1. **ConsoleApp.Simple** (Template 1)
- **Pattern**: Manual DI container setup
- **No Generic Host** - uses direct `ServiceCollection` instantiation
- **Inline service registration** - all services registered directly in Program.cs
- **Best for**: Learning, simple scripts, minimal overhead scenarios
- **Default async/await**: `false` (synchronous by default)

**Key files:**
- `Program.cs` - Entry point with manual DI setup
- `Services/MyService.cs` - Example service implementation
- `.template.config/template.json` - Template metadata

#### 2. **ConsoleApp.Standard** (Template 2)
- **Pattern**: Generic Host with BackgroundService
- **Uses**: `Host.CreateApplicationBuilder()`
- **Service registration**: Inline in Program.cs
- **Best for**: Production applications, long-running workers
- **Default async/await**: `true` (asynchronous by default)

**Key files:**
- `Program.cs` - Host setup with inline service registration
- `Workers/MyWorker.cs` - BackgroundService implementation
- `Services/MyService.cs` - Business logic service

#### 3. **ConsoleApp.Advanced** (Template 3)
- **Pattern**: Top-level statements + Extension methods
- **Uses**: Extension methods pattern for clean service registration
- **File**: `Extensions/ServiceExtensions.cs` (shared file)
- **Best for**: Medium to large applications, clean architecture
- **Default async/await**: `true` (asynchronous by default)

**Key files:**
- `Program.cs` - Minimal Program.cs using extension methods
- `Extensions/ServiceExtensions.cs` - Centralized service registration (SHARED)
- `Workers/MyWorker.cs` - BackgroundService implementation
- `Services/MyService.cs` - Business logic service

#### 4. **ConsoleApp.Enterprise** (Template 4)
- **Pattern**: Strongly-typed configuration + Options pattern
- **Uses**: Centralized configuration class
- **File**: `Configuration/ServiceConfiguration.cs` - Configuration orchestration
- **Best for**: Large applications, enterprise scenarios, multiple teams
- **Default async/await**: `true` (asynchronous by default)

**Key files:**
- `Program.cs` - Minimal Program.cs delegating to configuration
- `Configuration/ServiceConfiguration.cs` - Centralized service & configuration setup
- `Configuration/AppSettings.cs` - Application settings configuration
- `Configuration/WorkerSettings.cs` - Worker-specific settings
- `Workers/MyWorker.cs` - BackgroundService implementation
- `Services/MyService.cs` - Business logic service

### Template Configuration (`.template.config/template.json`)

Each template has a configuration file that defines:

**Essential metadata:**
```json
{
  "identity": "Inflop.ConsoleApp.Simple",
  "name": "Console App (Simple)",
  "shortName": "inflop-simple",
  "sourceName": "ConsoleApp.Simple"
}
```

**Template parameters (symbols):**
```json
{
  "symbols": {
    "Framework": {
      "type": "parameter",
      "datatype": "choice",
      "choices": [
        { "choice": "net8.0" },
        { "choice": "net9.0" },
        { "choice": "net10.0" }
      ],
      "defaultValue": "net8.0"
    },
    "UseAsync": {
      "type": "parameter",
      "datatype": "bool",
      "defaultValue": "false"
    }
  }
}
```

**Conditional compilation:**
- Symbols control which code is included in generated projects
- Uses special syntax: `//#if (Symbol)` ... `//#endif`
- Example: `//#if (AddSerilog)` includes Serilog-related code

---

## Shared Files System

### Why Shared Files?

Many files are **identical or nearly identical** across templates:
- API client implementations
- Database access patterns
- Messaging infrastructure
- Health check implementations
- Extension methods (for Advanced/Enterprise templates)

**Benefits:**
- Single source of truth - edit once, apply everywhere
- Consistency across templates
- Easier maintenance and bug fixes
- Namespace replacement handles template-specific namespaces

### Complete List of Shared Files

Files synchronized from `src/_shared/` to all applicable templates:

#### 1. **Template Configuration**
- `.template.config/dotnetcli.host.json`
  - Copied to: **All templates**
  - Purpose: CLI configuration for template engine

#### 2. **Extensions/** (Service Registration)
- `ServiceExtensions.cs`
  - Copied to: **Standard, Advanced, Enterprise** (NOT Simple)
  - Purpose: Centralized DI registration methods
  - Why excluded from Simple: Uses inline DI pattern

#### 3. **Infrastructure/**
- `DatabaseConnectionFactory.cs`
- Other infrastructure classes
  - Copied to: **All templates**
  - Purpose: Database connection management

#### 4. **Services/** (API Client)
- `IApiClient.cs`
- `ApiClient.cs`
  - Copied to: **All templates**
  - Purpose: HTTP client abstraction

#### 5. **Messaging/**
- Message queue implementations (RabbitMQ, Azure Service Bus, Kafka)
  - Copied to: **All templates**
  - Purpose: Messaging infrastructure

#### 6. **Health/**
- Health check implementations
  - Copied to: **All templates**
  - Purpose: Application health monitoring

#### 7. **Data/**
- `AppDbContext.cs` (EF Core)
- `DapperContext.cs` (Dapper)
- `Data/Models/*.cs` (Entity models)
  - Copied to: **All templates**
  - Purpose: Database access patterns

### Shared Files vs Template-Specific Files

| Category | Shared (edit in `_shared/`) | Template-Specific (edit directly) |
|----------|----------------------------|-----------------------------------|
| Template config | `.template.config/dotnetcli.host.json` | `.template.config/template.json` |
| Program entry | - | `Program.cs` |
| Worker/Service | - | `Workers/MyWorker.cs`, `Services/MyService.cs` |
| Service registration | `Extensions/ServiceExtensions.cs` | `Configuration/ServiceConfiguration.cs` (Enterprise only) |
| Configuration classes | - | `Configuration/*.cs` (Enterprise only) |
| Infrastructure | All `Infrastructure/*.cs` | - |
| API clients | `Services/IApiClient.cs`, `Services/ApiClient.cs` | - |
| Messaging | All `Messaging/*.cs` | - |
| Health checks | All `Health/*.cs` | - |
| Data access | All `Data/*.cs`, `Data/Models/*.cs` | - |

### How Synchronization Works (`sync-shared-files.sh`)

**What it does:**
1. Copies files from `src/_shared/` to each template directory
2. Replaces namespace `ConsoleApp.Advanced` with target namespace:
   - `ConsoleApp.Simple`
   - `ConsoleApp.Standard`
   - `ConsoleApp.Advanced`
   - `ConsoleApp.Enterprise`
3. Skips `Extensions/` for Simple template (uses inline DI)

**Example:**

```csharp
# Source file: src/_shared/Services/ApiClient.cs
namespace ConsoleApp.Advanced.Services { ... }

# After sync to Simple template:
namespace ConsoleApp.Simple.Services { ... }

# After sync to Enterprise template:
namespace ConsoleApp.Enterprise.Services { ... }
```

**Running the script:**

```bash
./scripts/sync-shared-files.sh
```

**Output:**

```bash
=========================================
Syncing shared files to all templates...
=========================================

📦 Syncing to: src/templates/1-ConsoleApp.Simple
  🎯 Target namespace: ConsoleApp.Simple
  ✓ Template config synced
  ✓ Infrastructure synced (namespace replaced)
  ✓ Services synced (namespace replaced)
  ...

✅ Sync complete!
```

---

## Development Workflows

### Workflow 1: Modifying Shared Files

**When to use:** You need to fix a bug or add a feature in files used by multiple templates (e.g., API client, database infrastructure).

**Steps:**

1. **Identify if the file is shared**

   ```bash
   # Check if file exists in _shared/
   ls src/_shared/Services/ApiClient.cs
   ```

2. **Edit the file in `src/_shared/`**

   ```bash
   # Edit shared file
   nano src/_shared/Services/ApiClient.cs

   # Make your changes using namespace ConsoleApp.Advanced
   namespace ConsoleApp.Advanced.Services
   {
       public class ApiClient : IApiClient
       {
           // Your changes here
       }
   }
   ```

3. **Run synchronization script**

   ```bash
   ./scripts/sync-shared-files.sh
   ```

   This copies changes to all templates and replaces namespaces.

4. **Verify changes in each template**

   ```bash
   # Check that changes were applied correctly
   cat src/templates/1-ConsoleApp.Simple/Services/ApiClient.cs
   cat src/templates/2-ConsoleApp.Standard/Services/ApiClient.cs
   ```

5. **Pack the template**

   ```bash
   dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release --no-build
   ```

6. **Test** (see [Testing Strategy](#testing-strategy))

**Important notes:**

- ALWAYS use namespace `ConsoleApp.Advanced` in shared files
- The sync script handles namespace replacement automatically
- NEVER edit shared files directly in template directories - changes will be overwritten!

### Workflow 2: Modifying Template-Specific Files

**When to use:** You need to change behavior specific to one template (e.g., Program.cs, Worker implementation).

**Steps:**

1. **Edit the file directly in the template directory**

   ```bash
   # Edit template-specific file
   nano src/templates/1-ConsoleApp.Simple/Program.cs
   ```

2. **Make changes considering conditional directives**

   ```csharp
   //#if (AddSerilog)
   using Serilog;
   //#endif

   // Your changes...
   ```

3. **Pack the template**

   ```bash
   dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release --no-build
   ```

4. **Test** (see [Testing Strategy](#testing-strategy))

**Common template-specific files:**

- `Program.cs` - Entry point (unique per template)
- `Workers/MyWorker.cs` - Worker implementation
- `Services/MyService.cs` - Example service
- `Configuration/ServiceConfiguration.cs` - Enterprise template only
- `Configuration/AppSettings.cs` - Enterprise template only
- `Configuration/WorkerSettings.cs` - Enterprise template only
- `.template.config/template.json` - Template metadata

### Workflow 3: Adding a New Template

**Steps:**

1. **Create new template directory**

   ```bash
   mkdir -p src/templates/5-ConsoleApp.NewTemplate
   cd src/templates/5-ConsoleApp.NewTemplate
   ```

2. **Create `.template.config/template.json`**

   ```json
   {
     "$schema": "http://json.schemastore.org/template",
     "author": "Inflop",
     "classifications": ["Console", "Worker"],
     "identity": "Inflop.ConsoleApp.NewTemplate",
     "name": "Console App (New Template)",
     "shortName": "inflop-new",
     "sourceName": "ConsoleApp.NewTemplate",
     "preferNameDirectory": true,
     "tags": {
       "language": "C#",
       "type": "project"
     },
     "symbols": {
       "Framework": {
         "type": "parameter",
         "datatype": "choice",
         "choices": [
           { "choice": "net8.0", "description": ".NET 8.0 (LTS)" },
           { "choice": "net9.0", "description": ".NET 9.0 (STS)" },
           { "choice": "net10.0", "description": ".NET 10.0 (LTS, latest)" }
         ],
         "defaultValue": "net8.0",
         "replaces": "net8.0",
         "description": "Target framework"
       }
     }
   }
   ```

3. **Create project file** (`ConsoleApp.NewTemplate.csproj`)

   ```xml
   <Project Sdk="Microsoft.NET.Sdk">
     <PropertyGroup>
       <OutputType>Exe</OutputType>
       <TargetFramework>net8.0</TargetFramework>
       <Nullable>enable</Nullable>
       <ImplicitUsings>enable</ImplicitUsings>
     </PropertyGroup>
   </Project>
   ```

4. **Create Program.cs** and other necessary files

5. **Update `sync-shared-files.sh`**

   ```bash
   TEMPLATES=(
       "src/templates/1-ConsoleApp.Simple"
       "src/templates/2-ConsoleApp.Standard"
       "src/templates/3-ConsoleApp.Advanced"
       "src/templates/4-ConsoleApp.Enterprise"
       "src/templates/5-ConsoleApp.NewTemplate"  # Add this line
   )

   # Add namespace mapping
   get_target_namespace() {
       # ...
       *"5-ConsoleApp.NewTemplate"*)
           echo "ConsoleApp.NewTemplate"
           ;;
   }
   ```

6. **Run sync to populate shared files**

   ```bash
   ./scripts/sync-shared-files.sh
   ```

7. **Update README.md** with template comparison matrix

8. **Test the new template**

### Workflow 4: Adding New Features/Parameters

**Example:** Adding a new optional feature (e.g., Redis caching)

**Steps:**

1. **Add symbol to `template.json`** (in each template or shared config)

   ```json
   {
     "symbols": {
       "AddRedis": {
         "type": "parameter",
         "datatype": "bool",
         "defaultValue": "false",
         "description": "Add Redis distributed caching support"
       }
     }
   }
   ```

2. **Add conditional package reference** (in .csproj)

   ```xml
   <!--#if (AddRedis) -->
   <PackageReference Include="StackExchange.Redis" Version="[2.8.0,)" />
   <!--#endif -->
   ```

3. **Add conditional code** (in Program.cs or shared files)

   ```csharp
   //#if (AddRedis)
   using StackExchange.Redis;
   //#endif

   // Service registration
   //#if (AddRedis)
   services.AddSingleton<IConnectionMultiplexer>(
       ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]!));
   //#endif
   ```

4. **Create shared implementation** (if applicable)

   ```bash
   # Create in _shared/ if used across templates
   vim src/_shared/Infrastructure/RedisCacheService.cs
   ```

5. **Run sync and test**

   ```bash
   ./scripts/sync-shared-files.sh
   dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release --no-build
   ```

6. **Test with and without the feature**

   ```bash
   dotnet new inflop-simple -n TestWithoutRedis
   dotnet new inflop-simple -n TestWithRedis --add-redis
   ```

---

## Template Engine & Conditional Directives

### Understanding Template Directives

.NET templates use **special comment syntax** for conditional compilation that is processed BEFORE C# compilation.

**Key concept:** These are NOT C# preprocessor directives (`#if`), they are **template engine directives** (`//#if`).

### Syntax

#### C# Files (`.cs`)

```csharp
//#if (Symbol)
using Some.Namespace;
//#endif

//#if (UseAsync)
public async Task ExecuteAsync()
{
    await DoSomethingAsync();
}
//#else
public void Execute()
{
    DoSomething();
}
//#endif
```

#### Project Files (`.csproj`)

```xml
<!--#if (AddSerilog) -->
<PackageReference Include="Serilog" Version="[4.0.0,)" />
<!--#endif -->
```

#### JSON/Text Files

```json
{
  //#if (UseDatabase)
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }
  //#endif
}
```

### Supported Operators

- `(Symbol)` - True if symbol is defined and true
- `(!Symbol)` - True if symbol is false or undefined
- `(Symbol1 && Symbol2)` - Logical AND
- `(Symbol1 || Symbol2)` - Logical OR
- `(Symbol == "value")` - String comparison

### Common Symbols in This Project

| Symbol | Type | Default | Purpose |
|--------|------|---------|---------|
| `Framework` | choice | net8.0 | Target framework selection |
| `UseAsync` | bool | varies | Enable async/await patterns |
| `AddSerilog` | bool | false | Include Serilog logging |
| `AddDocker` | bool | false | Include Dockerfile |
| `UseEfCore` | bool | false | Entity Framework Core |
| `UseDapper` | bool | false | Dapper micro-ORM |
| `DatabaseType` | choice | sqlite | Database provider |
| `AddHttpClient` | choice | none | HTTP client configuration |
| `UseHealthChecksBasic` | bool | false | Basic health checks |
| `UseHealthChecksAspNet` | bool | false | ASP.NET health checks |
| `UseRabbitMQ` | bool | false | RabbitMQ messaging |
| `UseAzureServiceBus` | bool | false | Azure Service Bus |
| `UseKafka` | bool | false | Apache Kafka |

### Why Templates Can't Be Built Directly

**Problem:**

```bash
cd src/templates/1-ConsoleApp.Simple
dotnet build  # ❌ FAILS!
```

**Error:**

```
error CS1024: Preprocessor directive expected
```

**Why?**
- C# compiler sees `//#if` as a comment followed by invalid syntax
- Template directives are processed by `dotnet new`, not the C# compiler
- Building directly tries to compile unprocessed template code

**Solution:**
- Pack the template WITHOUT building: `--no-build`
- Test by generating projects with `dotnet new`, THEN build the generated project

### Testing Conditional Directives

**Generate projects with different combinations:**
```bash
# Test default configuration
dotnet new inflop-simple -n Test1

# Test with async enabled
dotnet new inflop-simple -n Test2 --use-async true

# Test with Serilog
dotnet new inflop-simple -n Test3 --add-serilog

# Test with multiple features
dotnet new inflop-simple -n Test4 --use-async true --add-serilog --add-docker

# Test different frameworks
dotnet new inflop-simple -n Test5 -F net9.0

# Verify generated code
cd Test4
dotnet build  # ✅ Should succeed
dotnet run
```

---

## Building & Packaging

### Understanding Template Packaging

**The command:**
```bash
dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release
# ✅ Packs templates (automatically skips compilation)
```

**Why templates aren't compiled:**

The template project has special configuration in `.csproj`:
```xml
<IncludeBuildOutput>false</IncludeBuildOutput>
<PackageType>Template</PackageType>
```

This tells MSBuild to:
1. **Skip compilation** - Template files contain conditional directives (`//#if`, `//#endif`) that are processed by the template engine, not the C# compiler
2. **Package source files** - Templates are packaged as source code, not compiled assemblies
3. **Process during `dotnet new`** - Template engine processes directives when users create projects

**The workflow:**
1. Developer: `dotnet pack` → Creates .nupkg with template source files
2. User: `dotnet new install` → Installs template package
3. User: `dotnet new inflop-simple -n MyApp` → Template engine processes directives
4. User: `dotnet build` → User's generated project is compiled

### Complete Build Process

**Step 1: Make changes**
```bash
# Edit shared or template-specific files
nano src/_shared/Services/ApiClient.cs
```

**Step 2: Sync shared files (if applicable)**
```bash
./scripts/sync-shared-files.sh
```

**Step 3: Clean previous builds**
```bash
dotnet clean src/Inflop.ConsoleApp.Templates.csproj
rm -rf src/bin/Release/*.nupkg
```

**Step 4: Pack the template**
```bash
dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release --no-build
```

**Expected output:**
```
Microsoft (R) Build Engine version X.X.X
...
Successfully created package '/path/to/src/bin/Release/Inflop.ConsoleApp.Templates.1.0.0.nupkg'.
```

**Step 5: Verify package contents**
```bash
# Extract and inspect (optional)
unzip -l src/bin/Release/Inflop.ConsoleApp.Templates.1.0.0.nupkg
```

### Package Version Management

**Version is defined in .csproj:**
```xml
<Version>1.0.0</Version>
<PackageVersion>1.0.0</PackageVersion>
```

**Updating version:**
```bash
# Edit version in .csproj
nano src/Inflop.ConsoleApp.Templates.csproj

# Pack with new version
dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release --no-build
```

**Versioning strategy:**
- **Major** (1.x.x): Breaking changes, incompatible API changes
- **Minor** (x.1.x): New features, backward compatible
- **Patch** (x.x.1): Bug fixes, backward compatible

---

## Publishing to NuGet

### Overview

The project includes a GitHub Actions workflow for publishing template packages to NuGet.org. This workflow can be triggered manually on-demand and handles the complete publication process.

### Prerequisites

**Before publishing, you need to:**

1. **Create a NuGet API Key:**
   - Go to https://www.nuget.org/account/apikeys
   - Click "Create" to generate a new API key
   - Select appropriate scopes (typically "Push" and "Push new packages and package versions")
   - Set package glob pattern: `Inflop.ConsoleApp.Templates`
   - Copy the generated API key (you'll only see it once!)

2. **Add API Key to GitHub Secrets:**
   - Go to your GitHub repository
   - Navigate to Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `NUGET_API_KEY`
   - Value: Paste your NuGet API key
   - Click "Add secret"

### Publishing Workflow

The workflow is located at `.github/workflows/publish-nuget.yml`.

**To publish a new version:**

1. **Navigate to GitHub Actions:**
   - Go to your repository on GitHub
   - Click the "Actions" tab
   - Select "Publish to NuGet" workflow from the left sidebar

2. **Trigger the workflow:**
   - Click "Run workflow" button (top right)
   - Configure parameters:
     - **version**: (Optional) Enter a specific version (e.g., `1.0.1`). Leave empty to use version from `.csproj`
     - **prerelease**: Check if this is a pre-release version (prevents creating Git tag)
   - Click "Run workflow"

3. **Monitor the workflow:**
   - Watch the workflow execution in real-time
   - Check each step for success/failure
   - View the summary for package details

### What the Workflow Does

The publish workflow performs these steps:

1. **Checkout code** - Gets the latest code from the repository
2. **Setup .NET SDK** - Installs .NET 8.0 SDK
3. **Sync shared files** - Runs `sync-shared-files.sh` to ensure consistency
4. **Update version** (optional) - If version is specified, updates `.csproj`
5. **Pack template** - Creates `.nupkg` file in `./artifacts/`
6. **Validate package** - Ensures package file exists
7. **Publish to NuGet** - Pushes package to NuGet.org (skips duplicates)
8. **Create Git tag** - Tags the release (unless pre-release)
9. **Upload artifact** - Stores package as GitHub artifact (30 days retention)
10. **Publish summary** - Shows installation instructions and links

### Manual Publishing (Alternative)

You can also publish manually from your local machine:

```bash
# Step 1: Build the package
dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release

# Step 2: Publish to NuGet.org
dotnet nuget push src/bin/Release/Inflop.ConsoleApp.Templates.*.nupkg \
  --api-key YOUR_NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json

# Step 3: Create and push Git tag
git tag -a v1.0.1 -m "Release v1.0.1"
git push origin v1.0.1
```

**Security note:** Never commit your API key to the repository!

### Version Management

**Using version from .csproj (recommended):**

```bash
# Update version in .csproj
nano src/Inflop.ConsoleApp.Templates.csproj
# Change: <PackageVersion>1.0.1</PackageVersion>

# Commit the version change
git add src/Inflop.ConsoleApp.Templates.csproj
git commit -m "Bump version to 1.0.1"
git push

# Run workflow without version parameter
# (workflow will use version from .csproj)
```

**Overriding version via workflow input:**

```bash
# Run workflow with specific version
# Enter "1.0.1" in the version input field
# (workflow will update .csproj temporarily during build)
```

### Pre-release Versions

For beta/preview releases:

1. Use version format: `1.0.0-beta1`, `1.0.0-rc1`, `1.0.0-preview1`
2. Check "prerelease" option when running workflow
3. This prevents creating a Git release tag
4. Package will be marked as pre-release on NuGet

**Example:**
```
version: 1.1.0-beta1
prerelease: ✓ checked
```

### Post-Publication

After successful publication:

1. **Verify on NuGet:**
   - Go to https://www.nuget.org/packages/Inflop.ConsoleApp.Templates
   - Check that new version is listed
   - Verify package metadata (description, tags, etc.)

2. **Test installation:**
   ```bash
   # Install the published version
   dotnet new install Inflop.ConsoleApp.Templates::1.0.1

   # Verify templates are available
   dotnet new list | grep inflop

   # Test creating a project
   dotnet new inflop-simple -n TestPublished
   cd TestPublished && dotnet build && dotnet run
   ```

3. **Update documentation:**
   - Update README.md if new features were added
   - Create GitHub Release with release notes
   - Announce on relevant channels (if applicable)

### Troubleshooting

**Issue: "Package already exists" error**

The workflow uses `--skip-duplicate` flag, so this should be a warning, not an error. If you need to replace a package:
- You cannot replace an existing version on NuGet
- You must publish a new version (increment version number)

**Issue: "Authentication failed" error**

- Verify `NUGET_API_KEY` secret is set correctly in GitHub
- Check that API key hasn't expired
- Ensure API key has correct permissions (Push)

**Issue: "Package validation failed" error**

- Check that package metadata is valid (license, description, etc.)
- Ensure README.md exists and is included in package
- Verify package targets are correct

**Issue: Git tag creation failed**

- Check repository permissions
- Verify tag doesn't already exist: `git tag -l | grep v1.0.1`
- Ensure branch protection rules allow tag creation

### Best Practices

1. **Always test locally first** - Install and test package locally before publishing
2. **Use semantic versioning** - Follow SemVer (MAJOR.MINOR.PATCH)
3. **Update documentation** - Keep README and DEVELOPMENT.md in sync with changes
4. **Create release notes** - Document what's new in each version
5. **Tag releases** - Use Git tags to mark release points
6. **Test published package** - Always verify package works after publication

---

## Testing Strategy

### Testing Documentation

For comprehensive testing information, see:
- **`scripts/TEST-PLAN.md`** - Detailed test plan with 64 test cases covering all template variations
- **`scripts/TESTING-README.md`** - Quick start guide for running tests

### Automated Testing Scripts

The project includes several testing scripts:
- **`test-all-templates.sh`** - Runs comprehensive test suite (~64 tests, 15-25 minutes)
- **`test-single-template.sh`** - Tests a single template configuration
- **`show-template-help.sh`** - Displays available parameters for each template
- **`debug-test.sh`** - Debug helper for troubleshooting test failures

---

## Testing Strategy (Manual)

### Local Testing Workflow

**Complete test cycle:**

```bash
# 1. Uninstall previous version
dotnet new uninstall Inflop.ConsoleApp.Templates

# 2. Install from local package
dotnet new install src/bin/Release/Inflop.ConsoleApp.Templates.1.0.0.nupkg

# 3. Verify installation
dotnet new list | grep inflop
```

Expected output:
```
inflop-simple       Console App (Simple)       [C#]  Console/Worker
inflop-standard     Console App (Standard)     [C#]  Console/Worker
inflop-advanced     Console App (Advanced)     [C#]  Console/Worker
inflop-enterprise   Console App (Enterprise)   [C#]  Console/Worker
```

### Test Scenarios Matrix

**Test each template with multiple configurations:**

#### Template 1: Simple

```bash
# Default (synchronous, no features)
dotnet new inflop-simple -n Test_Simple_Default
cd Test_Simple_Default && dotnet build && dotnet run && cd ..

# Async enabled
dotnet new inflop-simple -n Test_Simple_Async --use-async true
cd Test_Simple_Async && dotnet build && dotnet run && cd ..

# With Serilog
dotnet new inflop-simple -n Test_Simple_Serilog --add-serilog
cd Test_Simple_Serilog && dotnet build && dotnet run && cd ..

# With Docker
dotnet new inflop-simple -n Test_Simple_Docker --add-docker
cd Test_Simple_Docker && dotnet build && cd ..

# With database (EF Core + SQLite)
dotnet new inflop-simple -n Test_Simple_EF --add-database efcore --database-type sqlite
cd Test_Simple_EF && dotnet build && cd ..

# With HTTP client + Polly
dotnet new inflop-simple -n Test_Simple_Http --add-httpclient with-polly
cd Test_Simple_Http && dotnet build && cd ..

# .NET 9.0
dotnet new inflop-simple -n Test_Simple_Net9 -F net9.0
cd Test_Simple_Net9 && dotnet build && cd ..
```

#### Template 2: Standard

```bash
# Default (asynchronous, BackgroundService)
dotnet new inflop-standard -n Test_Standard_Default
cd Test_Standard_Default && dotnet build && dotnet run && cd ..

# All features combined
dotnet new inflop-standard -n Test_Standard_Full \
    --add-serilog \
    --add-docker \
    --add-database efcore \
    --database-type postgres \
    --add-httpclient with-polly \
    --add-healthchecks aspnet

cd Test_Standard_Full && dotnet build && cd ..
```

#### Template 3: Advanced

```bash
# Default
dotnet new inflop-advanced -n Test_Advanced_Default
cd Test_Advanced_Default && dotnet build && dotnet run && cd ..

# With messaging (RabbitMQ)
dotnet new inflop-advanced -n Test_Advanced_RabbitMQ --add-messaging rabbitmq
cd Test_Advanced_RabbitMQ && dotnet build && cd ..
```

#### Template 4: Enterprise

```bash
# Default
dotnet new inflop-enterprise -n Test_Enterprise_Default
cd Test_Enterprise_Default && dotnet build && dotnet run && cd ..

# Full enterprise stack
dotnet new inflop-enterprise -n Test_Enterprise_Full \
    --add-serilog \
    --add-docker \
    --add-database efcore \
    --database-type sqlserver \
    --add-httpclient with-polly \
    --add-healthchecks aspnet \
    --add-messaging azureservicebus

cd Test_Enterprise_Full && dotnet build && cd ..
```

### Automated Testing Script

Create `test-all-templates.sh`:

```bash
#!/bin/bash
set -e

TEMPLATE_VERSION="1.0.0"
TEST_DIR="/tmp/template-tests-$(date +%s)"

echo "Creating test directory: $TEST_DIR"
mkdir -p "$TEST_DIR"
cd "$TEST_DIR"

# Install templates
dotnet new uninstall Inflop.ConsoleApp.Templates || true
dotnet new install ~/path/to/src/bin/Release/Inflop.ConsoleApp.Templates.$TEMPLATE_VERSION.nupkg

# Test each template
for template in inflop-simple inflop-standard inflop-advanced inflop-enterprise; do
    echo "========================================="
    echo "Testing: $template"
    echo "========================================="

    # Default configuration
    dotnet new $template -n Test_${template}_Default
    cd Test_${template}_Default
    dotnet restore
    dotnet build
    cd ..

    # With async (if applicable)
    if [ "$template" = "inflop-simple" ]; then
        dotnet new $template -n Test_${template}_Async --use-async true
        cd Test_${template}_Async
        dotnet build
        cd ..
    fi
done

echo "========================================="
echo "All tests completed successfully!"
echo "Test projects location: $TEST_DIR"
echo "========================================="
```

### Manual Verification Checklist

After generating test projects, verify:

- [ ] Project builds without errors
- [ ] Project runs without exceptions
- [ ] Namespaces are correctly replaced (no `ConsoleApp.Advanced` in Simple template)
- [ ] Conditional features work as expected
- [ ] Configuration files are present and valid (appsettings.json)
- [ ] Docker files are present (if `--add-docker` used)
- [ ] NuGet packages restore correctly
- [ ] Target framework matches selection (net8.0 or net9.0)
- [ ] Async/await patterns are correct (if enabled)
- [ ] No leftover template directives (`//#if` should not appear in generated code)

### Testing Edge Cases

**Test uncommon combinations:**

```bash
# Async disabled on Standard template
dotnet new inflop-standard -n EdgeCase1 --use-async false

# Multiple databases (should fail or warn)
# Test error handling in template.json

# Invalid framework
dotnet new inflop-simple -n EdgeCase2 -F net7.0
# Should fail or fallback to default
```

---

## Common Issues & Troubleshooting

### Issue 1: Template Build Fails

**Symptom:**

```bash
cd src/templates/1-ConsoleApp.Simple
dotnet build
# Error CS1024: Preprocessor directive expected
```

**Cause:**
Attempting to build template project directly. Template directives (`//#if`) are not valid C# code.

**Solution:**

- DO NOT build template projects directly
- Use `dotnet pack --no-build` to package templates
- Test by generating projects with `dotnet new`, then build generated projects

### Issue 2: Changes Not Reflected After Pack

**Symptom:**
Made changes to a file, packed the template, but generated projects still have old code.

**Possible causes & solutions:**

1. **Edited shared file in template directory instead of `_shared/`**
   - Solution: Edit in `src/_shared/`, run `./scripts/sync-shared-files.sh`, then pack

2. **Forgot to run sync script**
   - Solution: Always run `./scripts/sync-shared-files.sh` after editing shared files

3. **Old template still installed**

   ```bash
   dotnet new uninstall Inflop.ConsoleApp.Templates
   dotnet new install src/bin/Release/Inflop.ConsoleApp.Templates.1.0.0.nupkg
   ```

4. **Generated project cached**
   - Delete test project directory and regenerate

### Issue 3: Namespace Incorrect in Generated Project

**Symptom:**
Generated project from `inflop-simple` contains namespace `ConsoleApp.Advanced`.

**Cause:**
Shared file wasn't processed by sync script, or sync script failed silently.

**Solution:**

```bash
# Verify sync script works
./scripts/sync-shared-files.sh

# Check output for errors
# Verify file was copied and namespace replaced
cat src/templates/1-ConsoleApp.Simple/Services/ApiClient.cs | grep namespace
# Should show: namespace ConsoleApp.Simple.Services
```

### Issue 4: Template Installation Fails

**Symptom:**

```bash
dotnet new install src/bin/Release/Inflop.ConsoleApp.Templates.1.0.0.nupkg
# Error: Package does not exist
```

**Solution:**

```bash
# Verify package exists
ls -la src/bin/Release/*.nupkg

# Use absolute path
dotnet new install $(pwd)/src/bin/Release/Inflop.ConsoleApp.Templates.1.0.0.nupkg

# Or install from NuGet (if published)
dotnet new install Inflop.ConsoleApp.Templates
```

### Issue 5: Generated Project Build Fails

**Symptom:**
Generated project fails to build with missing namespace or type errors.

**Possible causes:**

1. **Template directives not processed correctly**
   - Check if `//#if` appears in generated code (shouldn't)
   - Verify template.json symbols are correct

2. **Missing package reference**
   - Check if conditional package reference works

   ```xml
   <!--#if (AddSerilog) -->
   <PackageReference Include="Serilog" Version="..." />
   <!--#endif -->
   ```

3. **Async/await mismatch**
   - Check if async methods are called correctly
   - Verify `UseAsync` symbol is working

**Debugging:**

```bash
# Generate with verbose output
dotnet new inflop-simple -n DebugTest --debug:custom

# Check generated files
find DebugTest -name "*.cs" -exec grep -l "//#if" {} \;
# Should return empty - no directives in generated code
```

### Issue 6: Sync Script Fails Silently

**Symptom:**
Script completes but files aren't updated.

**Solution:**

```bash
# Add verbose output
bash -x ./scripts/sync-shared-files.sh

# Check file permissions
ls -la src/_shared/Services/

# Verify template directories exist
ls -la src/templates/
```

### Issue 7: Package Version Conflict

**Symptom:**
Two versions of template installed, `dotnet new` uses wrong one.

**Solution:**

```bash
# List all installed templates
dotnet new uninstall

# Uninstall specific version
dotnet new uninstall Inflop.ConsoleApp.Templates

# Verify removal
dotnet new list | grep inflop
# Should return empty

# Install correct version
dotnet new install src/bin/Release/Inflop.ConsoleApp.Templates.1.0.0.nupkg
```

---

## Best Practices

### 1. Naming Conventions

**Template directories:**

- Pattern: `N-ConsoleApp.Name/` where N is the complexity order
- Examples: `1-ConsoleApp.Simple`, `2-ConsoleApp.Standard`

**Namespaces:**

- Pattern: `ConsoleApp.Name`
- Examples: `ConsoleApp.Simple`, `ConsoleApp.Enterprise`
- Use `ConsoleApp.Advanced` in shared files (sync script replaces)

**Template short names:**

- Pattern: `inflop-name` (lowercase, hyphenated)
- Examples: `inflop-simple`, `inflop-enterprise`

**Template identities:**

- Pattern: `Inflop.ConsoleApp.Name`
- Examples: `Inflop.ConsoleApp.Simple`, `Inflop.ConsoleApp.Enterprise`

### 2. Async/Await Patterns

**Default async behavior by template:**

- **Simple**: `false` (synchronous) - for beginners
- **Standard, Advanced, Enterprise**: `true` (asynchronous) - production-ready

**Implementing async/await:**

```csharp
//#if (UseAsync)
public async Task ExecuteAsync(CancellationToken cancellationToken)
{
    await DoWorkAsync(cancellationToken);
}
//#else
public void Execute()
{
    // ⚠️ WARNING: Synchronous blocking call
    // For production applications, consider using async patterns
    DoWorkAsync(CancellationToken.None).GetAwaiter().GetResult();
}
//#endif
```

**Best practices:**

- Always provide warning comments for synchronous blocking
- Use `CancellationToken` consistently
- Test both async and synchronous code paths

### 3. Package Versioning Strategy

**Package version ranges:**

```xml
<!-- ✅ Recommended: Range syntax -->
<PackageReference Include="Microsoft.Extensions.Hosting" Version="[8.0.0,)" />

<!-- ❌ Avoid: Fixed versions -->
<PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
```

**Why ranges?**

- Ensures minimum version (8.0.0) is used
- Allows patch updates (8.0.x) automatically
- Prevents breaking changes from major versions

**Range syntax explained:**

- `[8.0.0,)` - 8.0.0 or higher (recommended)
- `[8.0.0,9.0.0)` - 8.0.0 to 9.0.0 (exclusive)
- `8.0.0` - Exactly 8.0.0 (not recommended)

### 4. Configuration Management

**Configuration priority (highest to lowest):**

1. Command-line arguments
2. Environment variables
3. `appsettings.{Environment}.json`
4. `appsettings.json`

**Implementation:**

```csharp
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();
```

**Best practices:**

- Keep `reloadOnChange: false` for console apps (not long-running)
- Use `optional: true` to allow missing files
- Document required configuration keys

### 5. Template Symbol Naming

**Boolean symbols:**

- Pattern: `Add<Feature>` or `Use<Feature>`
- Examples: `AddSerilog`, `UseAsync`, `UseEfCore`

**Choice symbols:**

- Pattern: `<Feature>Type` or `<Feature>`
- Examples: `DatabaseType`, `Framework`, `AddHttpClient`

**Consistency rules:**

- Use PascalCase for symbol names
- Use descriptive names (not abbreviations)
- Provide clear descriptions in template.json

### 6. Shared vs Template-Specific Decision

**Use shared files when:**

- Code is identical or nearly identical across templates
- Namespace is the only difference
- Functionality is generic (API client, database access)

**Use template-specific files when:**

- Code differs significantly between templates
- File demonstrates template pattern (Program.cs, Worker implementation)
- File contains template-specific orchestration

**Examples:**

```text
Shared:
✅ Services/ApiClient.cs - identical logic
✅ Data/AppDbContext.cs - identical EF context
✅ Infrastructure/DatabaseConnectionFactory.cs - generic

Template-specific:
✅ Program.cs - different per template pattern
✅ Configuration/ServiceConfiguration.cs - Enterprise only
✅ Configuration/AppSettings.cs - Enterprise only
✅ Workers/MyWorker.cs - demonstrates template usage
```

### 7. Testing Before Release

**Pre-release checklist:**

- [ ] All templates generate without errors
- [ ] Generated projects build successfully
- [ ] Generated projects run without exceptions
- [ ] All feature combinations tested
- [ ] Both net8.0 and net9.0 tested
- [ ] Async and sync variants tested
- [ ] Documentation updated (README.md)
- [ ] Version number incremented appropriately
- [ ] Package metadata correct (authors, description, tags)

### 8. Documentation

**Keep documentation in sync:**

- Update CLAUDE.md / AGENTS.md for high-level changes
- Update DEVELOPMENT.md (this file) for detailed changes
- Update README.md for user-facing changes
- Document breaking changes prominently

**Good documentation practices:**

- Include examples for every feature
- Provide troubleshooting for common issues
- Keep table of contents updated
- Use clear, concise language

### 9. Git Workflow

**Before committing:**

```bash
# Verify no template build artifacts
git status | grep -E "(bin/|obj/)" && echo "⚠️  Remove build artifacts"

# Verify sync script was run (if applicable)
git diff src/_shared/
git diff src/templates/*/Services/ApiClient.cs

# Verify template.json changes are consistent
git diff src/templates/*/.template.config/template.json
```

**Commit messages:**

```
# Good commit messages
"Add Redis caching support to all templates"
"Fix namespace replacement in sync script"
"Update package versions to 8.0.1"

# Bad commit messages
"Update files"
"Fix bug"
"WIP"
```

### 10. Performance Considerations

**Template size:**

- Keep templates minimal - include only necessary files
- Use conditional directives for optional features
- Don't include large binary files in templates

**Sync script performance:**

- Script runs in seconds, even with multiple templates
- Namespace replacement is fast (simple sed operation)

---

## Appendix: Quick Reference Commands

```bash
# === DEVELOPMENT ===
# Sync shared files
./scripts/sync-shared-files.sh

# Pack templates
dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release

# === AUTOMATED TESTING ===
# Run comprehensive test suite (64 tests, ~15-25 minutes)
./scripts/test-all-templates.sh

# Test single template configuration
./scripts/test-single-template.sh inflop-simple MyTest --add-serilog

# Show all available parameters
./scripts/show-template-help.sh

# === MANUAL TESTING ===
# Uninstall/install locally
dotnet new uninstall Inflop.ConsoleApp.Templates
dotnet new install src/bin/Release/Inflop.ConsoleApp.Templates.*.nupkg

# List installed templates
dotnet new list | grep inflop

# Generate test project
dotnet new inflop-simple -n TestApp
cd TestApp && dotnet build && dotnet run

# === CLEANUP ===
# Remove build artifacts
dotnet clean src/Inflop.ConsoleApp.Templates.csproj
rm -rf src/bin/ src/obj/

# Remove test projects
rm -rf /tmp/inflop-template-tests-*
```

---

## Additional Resources

- [Official .NET Template Documentation](https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates)
- [Template JSON Schema](http://json.schemastore.org/template)
- [.NET Template Samples](https://github.com/dotnet/templating/wiki)

---

**Last updated:** 2025-10-20
**Maintained by:** Inflop Development Team
