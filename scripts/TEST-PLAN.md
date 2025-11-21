# Comprehensive Test Plan for Inflop.ConsoleApp.Templates

**Generated:** 2025-11-15
**Purpose:** Verify all templates work correctly with all parameter combinations

---

## Test Strategy

Testing all possible combinations (497,664 total) is impractical. This test plan uses a **focused approach**:

1. **Default configurations** - Verify each template works out-of-the-box
2. **Individual features** - Test each parameter/feature in isolation
3. **Framework variations** - Test both net8.0 and net9.0
4. **Complex combinations** - Test real-world scenarios with multiple features
5. **Edge cases** - Test unusual but valid combinations

---

## Test Execution

### Quick Start

```bash
# Make script executable
chmod +x scripts/test-all-templates.sh

# Run all tests
./scripts/test-all-templates.sh

# Results will be saved to: test-results-YYYYMMDD-HHMMSS.log
```

### Prerequisites

- .NET SDK 8.0 or 9.0 installed
- Bash shell (Linux/macOS or WSL on Windows)
- ~2GB free disk space for test projects
- Internet connection (for NuGet package restore)

### Expected Duration

- **~80-100 test cases**
- **Estimated time:** 15-25 minutes (depending on CPU and network speed)

---

## Test Coverage Matrix

### Templates Tested

| Template | Short Name | Default Async | Tests Count |
|----------|------------|---------------|-------------|
| Simple | inflop-simple | false (sync) | ~25 |
| Standard | inflop-standard | true (async) | ~15 |
| Advanced | inflop-advanced | true (async) | ~15 |
| Enterprise | inflop-enterprise | true (async) | ~15 |

### Parameters Tested

| Parameter | Values | Notes |
|-----------|--------|-------|
| Framework | net8.0, net9.0 | Both LTS and STS versions |
| UseAsync | true, false | All templates |
| AddSerilog | true, false | Logging integration |
| AddDocker | true, false | Docker support |
| AddCommandLine | none, system-commandline, spectre-console, command-line-parser | CLI parsing |
| AddDatabase | none, dapper, efcore | Database access |
| DatabaseType | sqlite, sqlserver, postgres | Only when AddDatabase != none |
| AddHttpClient | none, basic, with-polly | HTTP client support |
| AddHealthChecks | none, basic, aspnet | Health monitoring |
| AddMessaging | none, rabbitmq, azureservicebus, kafka | Message queues |

---

## Detailed Test Cases

### 1. INFLOP-SIMPLE Tests (~25 tests)

#### 1.1 Default & Framework Tests
- `Simple_Default` - Default configuration (net8.0, sync)
- `Simple_Net80` - Explicit net8.0 target
- `Simple_Net90` - Explicit net9.0 target

#### 1.2 Async/Await Tests
- `Simple_Async` - Async enabled (--use-async true)
- `Simple_Sync` - Sync explicit (--use-async false)

#### 1.3 Individual Features
- `Simple_Serilog` - Serilog logging
- `Simple_Docker` - Docker support

#### 1.4 Command-Line Parsers
- `Simple_SystemCommandLine` - System.CommandLine
- `Simple_SpectreConsole` - Spectre.Console
- `Simple_CommandLineParser` - CommandLineParser

#### 1.5 Database Variations (6 tests)
- `Simple_Dapper_SQLite` - Dapper + SQLite
- `Simple_Dapper_SqlServer` - Dapper + SQL Server
- `Simple_Dapper_Postgres` - Dapper + PostgreSQL
- `Simple_EFCore_SQLite` - Entity Framework Core + SQLite
- `Simple_EFCore_SqlServer` - EF Core + SQL Server
- `Simple_EFCore_Postgres` - EF Core + PostgreSQL

#### 1.6 HTTP Client Variations
- `Simple_HttpClient_Basic` - Basic typed HttpClient
- `Simple_HttpClient_Polly` - HttpClient with Polly resilience

#### 1.7 Health Checks Variations
- `Simple_HealthChecks_Basic` - Basic health checks
- `Simple_HealthChecks_AspNet` - ASP.NET health endpoint

#### 1.8 Messaging Variations
- `Simple_RabbitMQ` - RabbitMQ integration
- `Simple_AzureServiceBus` - Azure Service Bus
- `Simple_Kafka` - Apache Kafka

#### 1.9 Complex Combinations
- `Simple_Full_Net80` - Net8.0 + async + Serilog + Docker + EF Core + HttpClient
- `Simple_Full_Net90` - Net9.0 + all features enabled

**Expected Results:**
- All projects should generate without errors
- All projects should build successfully
- No unprocessed template directives (`//#if`, `//#endif`) in generated code
- Correct namespace replacement (`ConsoleApp.Simple`)

---

### 2. INFLOP-STANDARD Tests (~15 tests)

#### 2.1 Default & Framework Tests
- `Standard_Default` - Default configuration (net8.0, async)
- `Standard_Net80` - Explicit net8.0
- `Standard_Net90` - Explicit net9.0

#### 2.2 Async/Await Tests
- `Standard_Async` - Async enabled (default)
- `Standard_Sync` - Sync mode

#### 2.3 Individual Features
- `Standard_Serilog` - Serilog logging
- `Standard_Docker` - Docker support
- `Standard_Dapper_SQLite` - Dapper + SQLite
- `Standard_EFCore_Postgres` - EF Core + PostgreSQL
- `Standard_HttpClient_Polly` - HttpClient with Polly
- `Standard_HealthChecks_AspNet` - ASP.NET health checks
- `Standard_RabbitMQ` - RabbitMQ messaging

#### 2.4 Complex Combination
- `Standard_Full` - Net9.0 + all enterprise features

**Expected Results:**
- `Workers/MyWorker.cs` should implement BackgroundService
- Correct namespace replacement (`ConsoleApp.Standard`)
- Inline service registration in Program.cs

---

### 3. INFLOP-ADVANCED Tests (~15 tests)

#### 3.1 Default & Framework Tests
- `Advanced_Default` - Default configuration
- `Advanced_Net80` - Explicit net8.0
- `Advanced_Net90` - Explicit net9.0

#### 3.2 Async/Await Tests
- `Advanced_Async` - Async enabled
- `Advanced_Sync` - Sync mode

#### 3.3 Individual Features
- `Advanced_Serilog` - Serilog logging
- `Advanced_Docker` - Docker support
- `Advanced_Dapper_SQLite` - Dapper + SQLite
- `Advanced_EFCore_Postgres` - EF Core + PostgreSQL
- `Advanced_HttpClient_Polly` - HttpClient with Polly
- `Advanced_HealthChecks_AspNet` - ASP.NET health checks
- `Advanced_Kafka` - Apache Kafka

#### 3.4 Complex Combination
- `Advanced_Full` - Net9.0 + all features

**Expected Results:**
- `Extensions/ServiceExtensions.cs` should exist (extension methods pattern)
- Correct namespace replacement (`ConsoleApp.Advanced`)
- Top-level statements in Program.cs

---

### 4. INFLOP-ENTERPRISE Tests (~15 tests)

#### 4.1 Default & Framework Tests
- `Enterprise_Default` - Default configuration
- `Enterprise_Net80` - Explicit net8.0
- `Enterprise_Net90` - Explicit net9.0

#### 4.2 Async/Await Tests
- `Enterprise_Async` - Async enabled
- `Enterprise_Sync` - Sync mode

#### 4.3 Individual Features
- `Enterprise_Serilog` - Serilog logging
- `Enterprise_Docker` - Docker support
- `Enterprise_Dapper_SQLite` - Dapper + SQLite
- `Enterprise_EFCore_SqlServer` - EF Core + SQL Server
- `Enterprise_HttpClient_Polly` - HttpClient with Polly
- `Enterprise_HealthChecks_AspNet` - ASP.NET health checks
- `Enterprise_AzureServiceBus` - Azure Service Bus

#### 4.4 Complex Combination
- `Enterprise_Full` - Net9.0 + all enterprise features + Spectre.Console

**Expected Results:**
- `ServiceConfiguration.cs` should exist (centralized configuration)
- Strongly-typed configuration classes in `Configuration/` directory
- Correct namespace replacement (`ConsoleApp.Enterprise`)
- Options pattern implementation

---

## Validation Criteria

For each test case, the following checks are performed:

### 1. Template Generation
- ✅ `dotnet new` command succeeds
- ✅ All required files are created
- ✅ No error messages during generation

### 2. Package Restore
- ✅ `dotnet restore` succeeds
- ✅ All NuGet packages download successfully
- ✅ No version conflicts

### 3. Build Verification
- ✅ `dotnet build` succeeds
- ✅ No compilation errors
- ✅ No compiler warnings (except expected ones)

### 4. Code Quality Checks
- ✅ No unprocessed template directives in generated code
- ✅ Correct namespace replacement
- ✅ Proper indentation and formatting
- ✅ No hardcoded "ConsoleApp.Advanced" in non-Advanced templates

### 5. Feature-Specific Checks

**When AddSerilog = true:**
- ✅ Serilog packages in .csproj
- ✅ Serilog configuration in Program.cs

**When AddDocker = true:**
- ✅ Dockerfile exists
- ✅ .dockerignore exists
- ✅ docker-compose.yml exists

**When AddDatabase = dapper:**
- ✅ Dapper package reference
- ✅ `Infrastructure/DatabaseConnectionFactory.cs` exists
- ✅ No `Data/AppDbContext.cs`

**When AddDatabase = efcore:**
- ✅ EF Core package references
- ✅ `Data/AppDbContext.cs` exists
- ✅ Correct database provider package

**When AddHttpClient != none:**
- ✅ `Services/IApiClient.cs` exists
- ✅ `Services/ApiClient.cs` exists
- ✅ HttpClient registration in DI

**When AddHealthChecks = aspnet:**
- ✅ ASP.NET Core packages
- ✅ Health check endpoint configuration
- ✅ `Health/` directory exists

**When AddMessaging != none:**
- ✅ Messaging packages (RabbitMQ/Azure/Kafka)
- ✅ `Messaging/` directory exists
- ✅ Consumer and Publisher classes

---

## Known Issues & Expected Warnings

### 1. Synchronous Blocking Warnings
When `UseAsync = false` in templates 2-4, expect warnings like:
```
// ⚠️ WARNING: Synchronous blocking call
// For production applications, consider using async patterns
```

**Status:** Expected, by design

### 2. Database Connection String Warnings
Projects with database support will show:
```
Warning: Connection string 'DefaultConnection' not found
```

**Status:** Expected - users must configure connection strings

### 3. Messaging Configuration Warnings
Projects with messaging support may show:
```
Warning: RabbitMQ/Azure/Kafka connection not configured
```

**Status:** Expected - users must configure messaging endpoints

---

## Troubleshooting

### Test Failures

#### Build Errors
**Symptom:** `dotnet build` fails with compilation errors

**Common Causes:**
1. Template directives not processed (`//#if` in generated code)
2. Namespace replacement failed
3. Missing package references

**Solution:**
- Check test log file for detailed error messages
- Verify template.json symbols are correct
- Run `./scripts/sync-shared-files.sh` before packing

#### Template Generation Errors
**Symptom:** `dotnet new` fails to create project

**Common Causes:**
1. Template not installed correctly
2. Parameter name mismatch
3. Invalid parameter value

**Solution:**
- Verify template installation: `dotnet new list | grep inflop`
- Check parameter names match template.json
- Use `--help` flag: `dotnet new inflop-simple --help`

#### Restore Errors
**Symptom:** `dotnet restore` fails

**Common Causes:**
1. Network connectivity issues
2. NuGet package not found
3. Version conflicts

**Solution:**
- Check internet connection
- Verify package versions in .csproj
- Clear NuGet cache: `dotnet nuget locals all --clear`

### Manual Verification

If automated tests fail, manually verify:

```bash
# Generate project
dotnet new inflop-simple -n ManualTest

# Navigate to project
cd ManualTest

# Check for template directives (should return nothing)
grep -r "//#if\|//#endif" --include="*.cs" .

# Verify namespace
grep -r "namespace ConsoleApp.Simple" --include="*.cs" .

# Build
dotnet build

# Run
dotnet run
```

---

## Test Results Interpretation

### Success Criteria

- **100% pass rate:** All features working correctly ✅
- **95-99% pass rate:** Minor issues, acceptable for release ⚠️
- **<95% pass rate:** Major issues, do not release ❌

### Log Analysis

Test results are saved to `test-results-YYYYMMDD-HHMMSS.log`

**Key sections to review:**
```
[✓ PASS] - Successful tests
[✗ FAIL] - Failed tests (investigate immediately)
[WARN] - Warnings (review but may be expected)
[INFO] - Informational messages
```

**Example success:**
```
[INFO] Test #5: inflop-simple -> Simple_Async
[INFO] Parameters: --use-async true
[INFO] ✓ Template generation successful
[INFO] ✓ Restore successful
[INFO] ✓ Build successful
[INFO] ✓ Target framework: net8.0
[✓ PASS] Simple_Async: All checks passed
```

**Example failure:**
```
[INFO] Test #12: inflop-simple -> Simple_EFCore_Postgres
[INFO] Parameters: --add-database efcore --database-type postgres
[INFO] ✓ Template generation successful
[✗ FAIL] Simple_EFCore_Postgres: Build failed
```

---

## Post-Test Cleanup

Test directory (`/tmp/inflop-template-tests-*`) can be deleted after successful tests:

```bash
# Find test directories
ls -la /tmp/inflop-template-tests-*

# Remove specific test run
rm -rf /tmp/inflop-template-tests-1234567890

# Remove all test runs
rm -rf /tmp/inflop-template-tests-*
```

**Note:** Keep test directory if you need to investigate failures.

---

## Continuous Integration

### GitHub Actions Example

```yaml
name: Template Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: |
            8.0.x
            9.0.x

      - name: Run Template Tests
        run: |
          chmod +x scripts/test-all-templates.sh
          ./scripts/test-all-templates.sh

      - name: Upload Test Results
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: test-results
          path: test-results-*.log
```

---

## Appendix: Complete Test List

### inflop-simple (25 tests)
1. Simple_Default
2. Simple_Net80
3. Simple_Net90
4. Simple_Async
5. Simple_Sync
6. Simple_Serilog
7. Simple_Docker
8. Simple_SystemCommandLine
9. Simple_SpectreConsole
10. Simple_CommandLineParser
11. Simple_Dapper_SQLite
12. Simple_Dapper_SqlServer
13. Simple_Dapper_Postgres
14. Simple_EFCore_SQLite
15. Simple_EFCore_SqlServer
16. Simple_EFCore_Postgres
17. Simple_HttpClient_Basic
18. Simple_HttpClient_Polly
19. Simple_HealthChecks_Basic
20. Simple_HealthChecks_AspNet
21. Simple_RabbitMQ
22. Simple_AzureServiceBus
23. Simple_Kafka
24. Simple_Full_Net80
25. Simple_Full_Net90

### inflop-standard (13 tests)
1. Standard_Default
2. Standard_Net80
3. Standard_Net90
4. Standard_Async
5. Standard_Sync
6. Standard_Serilog
7. Standard_Docker
8. Standard_Dapper_SQLite
9. Standard_EFCore_Postgres
10. Standard_HttpClient_Polly
11. Standard_HealthChecks_AspNet
12. Standard_RabbitMQ
13. Standard_Full

### inflop-advanced (13 tests)
1. Advanced_Default
2. Advanced_Net80
3. Advanced_Net90
4. Advanced_Async
5. Advanced_Sync
6. Advanced_Serilog
7. Advanced_Docker
8. Advanced_Dapper_SQLite
9. Advanced_EFCore_Postgres
10. Advanced_HttpClient_Polly
11. Advanced_HealthChecks_AspNet
12. Advanced_Kafka
13. Advanced_Full

### inflop-enterprise (13 tests)
1. Enterprise_Default
2. Enterprise_Net80
3. Enterprise_Net90
4. Enterprise_Async
5. Enterprise_Sync
6. Enterprise_Serilog
7. Enterprise_Docker
8. Enterprise_Dapper_SQLite
9. Enterprise_EFCore_SqlServer
10. Enterprise_HttpClient_Polly
11. Enterprise_HealthChecks_AspNet
12. Enterprise_AzureServiceBus
13. Enterprise_Full

**Total: 64 comprehensive tests**

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-11-15 | Initial comprehensive test plan |

---

**Document Status:** Ready for Testing
**Maintainer:** Inflop Development Team
**Next Review:** After first test run
