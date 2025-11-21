# Run Tests Command

Execute comprehensive NuGet template testing and track results in TESTING_RESULTS.md.

## Your Task

You are to execute a comprehensive testing plan for NuGet templates. Follow these steps:

### 1. Preparation

**IMPORTANT:** Before starting any tests:
- Create a fresh TESTING_RESULTS.md file (overwrite if exists)
- Initialize the results file with a header and timestamp
- Create a TodoWrite list tracking all major phases of testing

### 2. Test Scenarios Specification

Execute **40 test scenarios** (4 templates × 10 scenarios each):

#### Template 1: inflop-simple

1. **Default Configuration** (net8.0, no optional features)
   - Command: `dotnet new inflop-simple -n Test_Simple_Default`
   - Expected: Build succeeds

2. **Framework net9.0**
   - Command: `dotnet new inflop-simple -n Test_Simple_Net9 -F net9.0`
   - Expected: Build succeeds with .NET 9.0

3. **With Serilog**
   - Command: `dotnet new inflop-simple -n Test_Simple_Serilog --add-serilog`
   - Expected: Build succeeds with Serilog logging

4. **With Docker**
   - Command: `dotnet new inflop-simple -n Test_Simple_Docker --add-docker`
   - Expected: Build succeeds, Dockerfile generated

5. **With CommandLine (System.CommandLine)**
   - Command: `dotnet new inflop-simple -n Test_Simple_CmdLine --add-commandline system-commandline`
   - Expected: Build succeeds with CLI parsing

6. **With Database EF Core + SQLite**
   - Command: `dotnet new inflop-simple -n Test_Simple_EF_SQLite --add-database efcore --database-type sqlite`
   - Expected: Build succeeds with EF Core

7. **With Database Dapper + PostgreSQL**
   - Command: `dotnet new inflop-simple -n Test_Simple_Dapper_Postgres --add-database dapper --database-type postgres`
   - Expected: Build succeeds with Dapper

8. **With HttpClient + Polly**
   - Command: `dotnet new inflop-simple -n Test_Simple_HttpPolly --add-httpclient with-polly`
   - Expected: Build succeeds with resilient HTTP client

9. **With HealthChecks ASP.NET**
   - Command: `dotnet new inflop-simple -n Test_Simple_HealthChecks --add-healthchecks aspnet`
   - Expected: Build succeeds with health checks endpoint

10. **With Messaging RabbitMQ**
    - Command: `dotnet new inflop-simple -n Test_Simple_RabbitMQ --add-messaging rabbitmq`
    - Expected: Build succeeds with RabbitMQ messaging

#### Template 2: inflop-standard

1. **Default Configuration** (net8.0, no optional features)
   - Command: `dotnet new inflop-standard -n Test_Standard_Default`
   - Expected: Build succeeds

2. **Framework net9.0**
   - Command: `dotnet new inflop-standard -n Test_Standard_Net9 -F net9.0`
   - Expected: Build succeeds with .NET 9.0

3. **With Serilog**
   - Command: `dotnet new inflop-standard -n Test_Standard_Serilog --add-serilog`
   - Expected: Build succeeds with Serilog logging

4. **With Docker**
   - Command: `dotnet new inflop-standard -n Test_Standard_Docker --add-docker`
   - Expected: Build succeeds, Dockerfile generated

5. **With CommandLine (System.CommandLine)**
   - Command: `dotnet new inflop-standard -n Test_Standard_CmdLine --add-commandline system-commandline`
   - Expected: Build succeeds with CLI parsing

6. **With Database EF Core + SQLite**
   - Command: `dotnet new inflop-standard -n Test_Standard_EF_SQLite --add-database efcore --database-type sqlite`
   - Expected: Build succeeds with EF Core

7. **With Database Dapper + PostgreSQL**
   - Command: `dotnet new inflop-standard -n Test_Standard_Dapper_Postgres --add-database dapper --database-type postgres`
   - Expected: Build succeeds with Dapper

8. **With HttpClient + Polly**
   - Command: `dotnet new inflop-standard -n Test_Standard_HttpPolly --add-httpclient with-polly`
   - Expected: Build succeeds with resilient HTTP client

9. **With HealthChecks ASP.NET**
   - Command: `dotnet new inflop-standard -n Test_Standard_HealthChecks --add-healthchecks aspnet`
   - Expected: Build succeeds with health checks endpoint

10. **With Messaging RabbitMQ**
    - Command: `dotnet new inflop-standard -n Test_Standard_RabbitMQ --add-messaging rabbitmq`
    - Expected: Build succeeds with RabbitMQ messaging

#### Template 3: inflop-advanced

1. **Default Configuration** (net8.0, no optional features)
   - Command: `dotnet new inflop-advanced -n Test_Advanced_Default`
   - Expected: Build succeeds

2. **Framework net9.0**
   - Command: `dotnet new inflop-advanced -n Test_Advanced_Net9 -F net9.0`
   - Expected: Build succeeds with .NET 9.0

3. **With Serilog**
   - Command: `dotnet new inflop-advanced -n Test_Advanced_Serilog --add-serilog`
   - Expected: Build succeeds with Serilog logging

4. **With Docker**
   - Command: `dotnet new inflop-advanced -n Test_Advanced_Docker --add-docker`
   - Expected: Build succeeds, Dockerfile generated

5. **With CommandLine (System.CommandLine)**
   - Command: `dotnet new inflop-advanced -n Test_Advanced_CmdLine --add-commandline system-commandline`
   - Expected: Build succeeds with CLI parsing

6. **With Database EF Core + SQLite**
   - Command: `dotnet new inflop-advanced -n Test_Advanced_EF_SQLite --add-database efcore --database-type sqlite`
   - Expected: Build succeeds with EF Core

7. **With Database Dapper + PostgreSQL**
   - Command: `dotnet new inflop-advanced -n Test_Advanced_Dapper_Postgres --add-database dapper --database-type postgres`
   - Expected: Build succeeds with Dapper

8. **With HttpClient + Polly**
   - Command: `dotnet new inflop-advanced -n Test_Advanced_HttpPolly --add-httpclient with-polly`
   - Expected: Build succeeds with resilient HTTP client

9. **With HealthChecks ASP.NET**
   - Command: `dotnet new inflop-advanced -n Test_Advanced_HealthChecks --add-healthchecks aspnet`
   - Expected: Build succeeds with health checks endpoint

10. **With Messaging RabbitMQ**
    - Command: `dotnet new inflop-advanced -n Test_Advanced_RabbitMQ --add-messaging rabbitmq`
    - Expected: Build succeeds with RabbitMQ messaging

#### Template 4: inflop-enterprise

1. **Default Configuration** (net8.0, no optional features)
   - Command: `dotnet new inflop-enterprise -n Test_Enterprise_Default`
   - Expected: Build succeeds

2. **Framework net9.0**
   - Command: `dotnet new inflop-enterprise -n Test_Enterprise_Net9 -F net9.0`
   - Expected: Build succeeds with .NET 9.0

3. **With Serilog**
   - Command: `dotnet new inflop-enterprise -n Test_Enterprise_Serilog --add-serilog`
   - Expected: Build succeeds with Serilog logging

4. **With Docker**
   - Command: `dotnet new inflop-enterprise -n Test_Enterprise_Docker --add-docker`
   - Expected: Build succeeds, Dockerfile generated

5. **With CommandLine (System.CommandLine)**
   - Command: `dotnet new inflop-enterprise -n Test_Enterprise_CmdLine --add-commandline system-commandline`
   - Expected: Build succeeds with CLI parsing

6. **With Database EF Core + SQLite**
   - Command: `dotnet new inflop-enterprise -n Test_Enterprise_EF_SQLite --add-database efcore --database-type sqlite`
   - Expected: Build succeeds with EF Core

7. **With Database Dapper + PostgreSQL**
   - Command: `dotnet new inflop-enterprise -n Test_Enterprise_Dapper_Postgres --add-database dapper --database-type postgres`
   - Expected: Build succeeds with Dapper

8. **With HttpClient + Polly**
   - Command: `dotnet new inflop-enterprise -n Test_Enterprise_HttpPolly --add-httpclient with-polly`
   - Expected: Build succeeds with resilient HTTP client

9. **With HealthChecks ASP.NET**
   - Command: `dotnet new inflop-enterprise -n Test_Enterprise_HealthChecks --add-healthchecks aspnet`
   - Expected: Build succeeds with health checks endpoint

10. **With Messaging RabbitMQ**
    - Command: `dotnet new inflop-enterprise -n Test_Enterprise_RabbitMQ --add-messaging rabbitmq`
    - Expected: Build succeeds with RabbitMQ messaging

### 3. Execute Testing Phases

**Phase 1: Package Preparation**
Execute these steps sequentially:
1. Clean previous builds: `dotnet clean src/Inflop.ConsoleApp.Templates.csproj`
2. Build solution: `dotnet build src/Inflop.ConsoleApp.Templates.sln` (build errors are expected - templates contain conditional directives)
3. Pack templates: `dotnet pack src/Inflop.ConsoleApp.Templates.csproj -c Release --no-build`
4. Uninstall old version: `dotnet new uninstall Inflop.ConsoleApp.Templates` (ignore errors if not installed)
5. Install fresh package from local path (find the .nupkg in src/bin/Release/)

Document each step's result in TESTING_RESULTS.md immediately after execution.

**Phase 2: Template Testing**
For each of the 4 templates (inflop-simple, inflop-standard, inflop-advanced, inflop-enterprise):
- Execute all 10 test scenarios as defined above in section 2
- For each scenario:
  1. Mark the scenario as "in_progress" in your TodoWrite list
  2. Execute the `dotnet new` command in a temporary test directory
  3. Run `dotnet build` on the generated project
  4. Capture the build output (both stdout and stderr)
  5. Analyze the results:
     - **SUCCESS:** Build succeeded without errors
     - **FAILURE:** Build failed with compilation errors
  6. Document the result immediately in TESTING_RESULTS.md with:
     - Test scenario name
     - Command executed
     - Build result (SUCCESS/FAILURE)
     - If FAILURE: Full error details, error codes, affected files
     - Timestamp
  7. Mark the scenario as "completed" in TodoWrite
  8. Clean up the test directory before next test

**Phase 3: Problem Analysis and Fixes**
When encountering build failures:
1. Document the problem in TESTING_RESULTS.md under a "Problems Encountered" section
2. Analyze the root cause by examining:
   - Error messages and error codes
   - Template configuration files (.template.config/template.json)
   - Generated source files
   - Conditional compilation directives
3. If you can identify a fix:
   - Document the proposed solution in TESTING_RESULTS.md
   - Ask user for permission to apply the fix
   - After user approval, apply the fix
   - Re-run the failed test scenario
   - Document whether the fix resolved the issue
4. If you cannot identify a fix:
   - Document what you investigated
   - Suggest next steps for manual investigation

### 4. Results Documentation Format

The TESTING_RESULTS.md file should follow this structure:

```markdown
# NuGet Template Testing Results

**Test Run Date:** [timestamp]
**Template Package Version:** [version from .csproj]
**Test Executor:** Claude Code (run-tests command)

## Executive Summary

- Total Scenarios: X
- Passed: Y (Z%)
- Failed: W (V%)
- Templates Tested: 4 (inflop-simple, inflop-standard, inflop-advanced, inflop-enterprise)

## Preparation Phase

### Step 1: Clean Build
- Command: [command]
- Result: [SUCCESS/FAILURE]
- Output: [relevant output]

[... continue for all prep steps]

## Template Testing Results

### inflop-simple

#### Scenario 1: Default Configuration
- Command: `dotnet new inflop-simple -n Test_Simple_Default`
- Build Command: `dotnet build`
- Result: ✅ SUCCESS / ❌ FAILURE
- Duration: [time]
- Output: [relevant output or "Build succeeded"]
- Errors: [if any]

[... continue for all scenarios]

### inflop-standard
[... same structure]

### inflop-advanced
[... same structure]

### inflop-enterprise
[... same structure]

## Problems Encountered

### Problem #1: [Short Description]
- Template: [template name]
- Scenario: [scenario name]
- Error Message: [full error]
- Root Cause: [analysis]
- Solution Status: ✅ RESOLVED / ⏳ IN PROGRESS / ❌ UNRESOLVED
- Fix Applied: [description if resolved]
- Verification: [re-test result if resolved]

[... continue for all problems]

## Summary Statistics

- inflop-simple: X/10 scenarios passed (Y%)
- inflop-standard: X/10 scenarios passed (Y%)
- inflop-advanced: X/10 scenarios passed (Y%)
- inflop-enterprise: X/10 scenarios passed (Y%)

## Recommendations

[Your analysis and recommendations based on test results]
```

### 5. Best Practices

- Update TESTING_RESULTS.md **immediately** after each test scenario (don't wait until all tests complete)
- Use clear SUCCESS/FAILURE markers (✅/❌)
- Include full error messages for failures (they're critical for debugging)
- Keep TodoWrite list updated so user can track progress in real-time
- Clean up test directories after each test to avoid conflicts
- If a template completely fails, document it but continue with other templates
- Run tests in a temporary directory (suggest: `/tmp/template-tests-[timestamp]`)

### 6. Error Handling

- If package installation fails, stop and report the issue
- If a template fails, document it and continue with remaining scenarios
- If build errors occur, capture full output including warnings
- Handle expected build errors in template source (mention these are expected during pack phase)

### 7. Final Report

After all tests complete:
1. Generate a final summary in TESTING_RESULTS.md
2. Calculate success rates for each template
3. List all unresolved problems
4. Provide recommendations for next steps

## Important Notes

- Tests must be run from the project root directory: `/mnt/home/rk/Projekty/Inflop.ConsoleApp.Templates`
- Use `--no-build` flag when packing to avoid building template source files
- Template source files contain conditional directives (`//#if`) that cause build errors when compiled directly
- Only generated projects from `dotnet new` should be built and tested
- Maintain a clean test environment by removing test directories after each scenario
