# NuGet Publication Guide

## Package Information

**Package Name:** Inflop.ConsoleApp.Templates
**Version:** 1.0.0
**License:** MIT
**Authors:** Inflop

---

## Prerequisites

1. **NuGet Account:**
   - Create account at https://www.nuget.org/
   - Generate API Key at https://www.nuget.org/account/apikeys

2. **dotnet CLI:**

   ```bash
   dotnet --version  # Verify .NET SDK is installed
   ```

---

## Publishing Steps

### 1. Build the Package

```bash
cd /mnt/home/rk/Projekty/Inflop.ConsoleApp.Templates/src
dotnet pack -c Release --no-build
```

**Output:** `bin/Release/Inflop.ConsoleApp.Templates.1.0.0.nupkg`

---

### 2. Test Package Locally

```bash
# Uninstall previous version
dotnet new uninstall Inflop.ConsoleApp.Templates

# Install from local package
dotnet new install bin/Release/Inflop.ConsoleApp.Templates.1.0.0.nupkg

# Verify templates are available
dotnet new list | grep console

# Test template generation
dotnet new console-modern -n TestProject \
  --add-database efcore -D sqlite \
  --add-messaging rabbitmq \
  --add-healthchecks basic

# Build test project
cd TestProject && dotnet build
```

---

### 3. Publish to NuGet.org

```bash
# Set your API key (one-time setup)
export NUGET_API_KEY="your-api-key-here"

# Push to NuGet.org
dotnet nuget push bin/Release/Inflop.ConsoleApp.Templates.1.0.0.nupkg \
  --api-key $NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

**Alternative (interactive):**

```bash
dotnet nuget push bin/Release/Inflop.ConsoleApp.Templates.1.0.0.nupkg \
  --source https://www.nuget.org
# You'll be prompted for API key
```

---

### 4. Verify Publication

1. Wait 5-10 minutes for indexing
2. Check at: https://www.nuget.org/packages/Inflop.ConsoleApp.Templates/
3. Test installation from NuGet:

```bash
dotnet new uninstall Inflop.ConsoleApp.Templates
dotnet new install Inflop.ConsoleApp.Templates
dotnet new list | grep console
```

---

## Package Metadata

### Description (from .csproj)

> A collection of .NET console application templates with DI, configuration, and logging support. Includes 4 templates: Simple (manual DI), Standard (Generic Host), Advanced (top-level statements), and Enterprise (Options pattern).

### Tags

`dotnet-new`, `templates`, `console`, `di`, `configuration`, `logging`, `database`, `messaging`, `docker`, `healthchecks`

### URLs

- **Project URL:** https://github.com/inflop/Inflop.ConsoleApp.Templates
- **Repository:** https://github.com/inflop/Inflop.ConsoleApp.Templates
- **Issues:** https://github.com/inflop/Inflop.ConsoleApp.Templates/issues
- **License:** MIT

---

## Package Features (for NuGet listing)

### Core Templates
- ✅ 4 console application templates (Simple, Standard, Advanced, Enterprise)
- ✅ Multi-version support (.NET 8.0, 9.0, 10.0)
- ✅ Built-in DI, Configuration, and Logging

### Advanced Parameters
- ✅ **Database:** EF Core or Dapper (SQLite, SQL Server, PostgreSQL)
- ✅ **Messaging:** RabbitMQ, Azure Service Bus, Apache Kafka
- ✅ **HTTP Client:** Typed client with Polly resilience
- ✅ **Health Checks:** Automatic dependency monitoring
- ✅ **CLI Parsing:** System.CommandLine, Spectre.Console, CommandLineParser
- ✅ **Docker:** Multi-stage Dockerfile + docker-compose
- ✅ **Logging:** Serilog structured logging

---

## Installation (for users)

```bash
# Install from NuGet
dotnet new install Inflop.ConsoleApp.Templates

# Verify installation
dotnet new list

# Create project
dotnet new console-modern -n MyApp

# With advanced features
dotnet new console-modern -n MyApp \
  --AddDatabase efcore -D postgres \
  --AddMessaging rabbitmq \
  --AddHealthChecks aspnet \
  --AddDocker \
  --AddSerilog
```

---

## Release Checklist

- [x] All 4 templates tested
- [x] Advanced parameters work correctly
- [x] Selective file exclusion verified
- [x] README.md updated with full documentation
- [x] Package built successfully
- [x] Local testing passed
- [ ] API key configured
- [ ] Published to NuGet.org
- [ ] Verified on NuGet.org
- [ ] Announced release

---

## Version Updates (Future)

To publish a new version:

1. Update version in `Inflop.ConsoleApp.Templates.csproj`:
   ```xml
   <PackageVersion>1.1.0</PackageVersion>
   ```

2. Update `VERSION_HISTORY` in README.md

3. Rebuild and republish:
   ```bash
   dotnet pack -c Release
   dotnet nuget push bin/Release/Inflop.ConsoleApp.Templates.1.1.0.nupkg \
     --api-key $NUGET_API_KEY \
     --source https://api.nuget.org/v3/index.json
   ```

---

## Support

After publication, monitor:
- NuGet.org download statistics
- GitHub issues for bug reports
- User feedback and feature requests

**Documentation:** https://github.com/inflop/Inflop.ConsoleApp.Templates/blob/main/README.md
