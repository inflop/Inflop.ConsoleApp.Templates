# Testing Inflop.ConsoleApp.Templates

Quick guide for running comprehensive template tests.

## Quick Start

```bash
# 1. Make script executable (if not already)
chmod +x scripts/test-all-templates.sh

# 2. Run all tests
./scripts/test-all-templates.sh
```

## What Gets Tested

The script tests all 4 templates with ~64 different parameter combinations:

- **inflop-simple** - 25 test cases
- **inflop-standard** - 13 test cases
- **inflop-advanced** - 13 test cases
- **inflop-enterprise** - 13 test cases

### Test Coverage

Each template is tested with:

- ✅ Default configuration
- ✅ Both frameworks (net8.0, net9.0)
- ✅ Async/sync variations
- ✅ All individual features (Serilog, Docker, Database, HTTP Client, etc.)
- ✅ Complex multi-feature combinations

## Prerequisites

- .NET SDK 8.0 or 9.0
- Bash shell (Linux/macOS/WSL)
- ~2GB free disk space
- Internet connection

## Expected Duration

**15-25 minutes** depending on CPU and network speed

## Test Results

Results are saved to: `test-results-YYYYMMDD-HHMMSS.log`

### Success Indicators

```bash
[✓ PASS] ProjectName: All checks passed
```

### Failure Indicators

```bash
[✗ FAIL] ProjectName: Build failed
[✗ FAIL] ProjectName: Template generation failed
```

## Test Process

For each test case, the script:

1. ✅ Generates project from template with specified parameters
2. ✅ Runs `dotnet restore`
3. ✅ Runs `dotnet build`
4. ✅ Verifies no template directives in generated code
5. ✅ Checks target framework

## Manual Testing

If you want to test a specific configuration manually:

```bash
# 1. Pack templates
dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release --no-build

# 2. Install
dotnet new uninstall Inflop.ConsoleApp.Templates
dotnet new install src/bin/Release/Inflop.ConsoleApp.Templates.*.nupkg

# 3. Generate project
dotnet new inflop-simple -n MyTest --add-serilog --add-docker

# 4. Build and run
cd MyTest
dotnet build
dotnet run
```

## Test Parameter Examples

### Simple Template

```bash
# Default
dotnet new inflop-simple -n Test1

# With Serilog
dotnet new inflop-simple -n Test2 --add-serilog

# With database
dotnet new inflop-simple -n Test3 --add-database efcore --database-type sqlite

# Full featured
dotnet new inflop-simple -n Test4 \
  -F net9.0 \
  --use-async true \
  --add-serilog \
  --add-docker \
  --add-database efcore \
  --database-type postgres \
  --add-http-client with-polly \
  --add-health-checks aspnet
```

### Other Templates

```bash
# Standard
dotnet new inflop-standard -n MyStandard --add-messaging rabbitmq

# Advanced
dotnet new inflop-advanced -n MyAdvanced --add-database dapper

# Enterprise
dotnet new inflop-enterprise -n MyEnterprise \
  --add-database efcore \
  --database-type sqlserver \
  --add-messaging azureservicebus
```

## Available Parameters

| Parameter | Values | Description |
|-----------|--------|-------------|
| -F | net8.0, net9.0 | Target framework |
| --use-async | true/false | Enable async/await |
| --add-serilog | (flag) | Add Serilog logging |
| --add-docker | (flag) | Add Docker support |
| --add-command-line | none, system-commandline, spectre-console, command-line-parser | CLI parsing |
| --add-database | none, dapper, efcore | Database access |
| --database-type | sqlite, sqlserver, postgres | Database provider |
| --add-http-client | none, basic, with-polly | HTTP client |
| --add-health-checks | none, basic, aspnet | Health checks |
| --add-messaging | none, rabbitmq, azureservicebus, kafka | Messaging |

## Troubleshooting

### Tests Fail to Start

**Problem:** Script exits immediately

**Solution:**

```bash
# Verify .NET SDK installed
dotnet --version

# Should show 8.0.x or 9.0.x
```

### Build Errors

**Problem:** Generated projects fail to build

**Solution:**

```bash
# Check test log
cat test-results-*.log | grep "FAIL"

# Common fixes:
# 1. Run sync script before packing
./scripts/sync-shared-files.sh

# 2. Clean and repack
dotnet clean src/Inflop.ConsoleApp.Templates.csproj
dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release --no-build
```

### Template Not Found

**Problem:** `dotnet new: Template 'inflop-simple' not found`

**Solution:**

```bash
# Verify installation
dotnet new list | grep inflop

# Reinstall if needed
dotnet new uninstall Inflop.ConsoleApp.Templates
dotnet new install src/bin/Release/Inflop.ConsoleApp.Templates.*.nupkg
```

## Cleanup

Remove test projects after successful run:

```bash
# Test projects are created in /tmp/inflop-template-tests-*
rm -rf /tmp/inflop-template-tests-*
```

## Detailed Documentation

For complete test documentation, see: **TEST-PLAN.md**

## CI/CD Integration

The test script can be integrated into CI/CD pipelines:

```yaml
# GitHub Actions example
- name: Run Template Tests
  run: |
    chmod +x scripts/test-all-templates.sh
    ./scripts/test-all-templates.sh

- name: Upload Results
  uses: actions/upload-artifact@v3
  with:
    name: test-results
    path: test-results-*.log
```

## Questions?

- Check detailed documentation: `scripts/TEST-PLAN.md`
- Review development guide: `DEVELOPMENT.md`
- See user documentation: `README.md`

---

**Good luck with testing! 🚀**
