#!/bin/bash

# Comprehensive Template Testing Script
# Tests all 4 templates with key parameter combinations

set -e  # Exit on error
set -o pipefail  # Exit on pipe failure

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Counters
TOTAL_TESTS=0
PASSED_TESTS=0
FAILED_TESTS=0

# Test results file (use absolute path to avoid issues with directory changes)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TEST_RESULTS_FILE="$SCRIPT_DIR/test-results-$(date +%Y%m%d-%H%M%S).log"
TEST_DIR="/tmp/inflop-template-tests-$(date +%s)"

# Log function - force output to stderr (unbuffered)
log() {
    echo -e "${BLUE}[INFO]${NC} $1" >&2
    echo "[INFO] $1" >> "$TEST_RESULTS_FILE"
}

log_success() {
    echo -e "${GREEN}[✓ PASS]${NC} $1" >&2
    echo "[✓ PASS] $1" >> "$TEST_RESULTS_FILE"
    PASSED_TESTS=$((PASSED_TESTS + 1))
}

log_error() {
    echo -e "${RED}[✗ FAIL]${NC} $1" >&2
    echo "[✗ FAIL] $1" >> "$TEST_RESULTS_FILE"
    FAILED_TESTS=$((FAILED_TESTS + 1))
}

log_warning() {
    echo -e "${YELLOW}[WARN]${NC} $1" >&2
    echo "[WARN] $1" >> "$TEST_RESULTS_FILE"
}

# Test a single template configuration
test_template() {
    local template_name=$1
    local project_name=$2
    shift 2
    local params=("$@")

    TOTAL_TESTS=$((TOTAL_TESTS + 1))

    log "========================================="
    log "Test #$TOTAL_TESTS: $template_name -> $project_name"
    log "Parameters: ${params[*]}"
    log "========================================="

    # Create project from template
    cd "$TEST_DIR"
    log "  → Generating project..."
    echo "Executing: dotnet new $template_name -n $project_name ${params[*]}" >> "$TEST_RESULTS_FILE"
    if dotnet new "$template_name" -n "$project_name" "${params[@]}" 2>&1 | tee -a "$TEST_RESULTS_FILE"; then
        echo "✓ Generation successful" >> "$TEST_RESULTS_FILE"
        log "✓ Template generation successful"
    else
        log_error "$project_name: Template generation failed"
        return 1
    fi

    # Build project
    cd "$project_name"

    # Restore packages
    log "  → Restoring packages..."
    echo "Executing: dotnet restore" >> "$TEST_RESULTS_FILE"
    if dotnet restore 2>&1 | tee -a "$TEST_RESULTS_FILE"; then
        log "✓ Restore successful"
    else
        log_error "$project_name: Restore failed"
        cd "$TEST_DIR"
        return 1
    fi

    # Build project
    log "  → Building project..."
    echo "Executing: dotnet build" >> "$TEST_RESULTS_FILE"
    if dotnet build 2>&1 | tee -a "$TEST_RESULTS_FILE"; then
        log "✓ Build successful"
    else
        log_error "$project_name: Build failed"
        cd "$TEST_DIR"
        return 1
    fi

    # Check for template directives in generated code (shouldn't exist)
    if grep -r "//#if\|//#endif" --include="*.cs" . >> "$TEST_RESULTS_FILE" 2>&1; then
        log_warning "$project_name: Found unprocessed template directives"
    fi

    # Verify target framework
    if [ -f "*.csproj" ]; then
        local framework=$(grep -oP '(?<=<TargetFramework>)[^<]+' *.csproj | head -1)
        log "✓ Target framework: $framework"
    fi

    log_success "$project_name: All checks passed"
    cd "$TEST_DIR"
    return 0
}

# Main test execution
main() {
    log "========================================="
    log "Inflop.ConsoleApp.Templates - Comprehensive Test Suite"
    log "Started: $(date)"
    log "Test directory: $TEST_DIR"
    log "========================================="

    # Create test directory
    mkdir -p "$TEST_DIR"

    # Step 0: Sync shared files to all templates
    log "Step 0: Synchronizing shared files to all templates..."
    log "  - Running: ./sync-shared-files.sh"
    if ./sync-shared-files.sh >> "$TEST_RESULTS_FILE" 2>&1; then
        log "✓ Shared files synchronized successfully"
    else
        log_error "Failed to sync shared files"
        exit 1
    fi

    # Step 1: Pack templates
    log "Step 1: Packing templates..."
    cd "$(dirname "$0")"

    # Clean previous builds
    log "  - Cleaning previous builds..."
    rm -rf ../src/bin/Release/*.nupkg 2>/dev/null || true

    # Pack templates (IncludeBuildOutput=false in .csproj prevents compilation)
    log "  - Running: dotnet pack ../src/Inflop.ConsoleApp.Templates.csproj -c Release"
    echo "Executing: dotnet pack ../src/Inflop.ConsoleApp.Templates.csproj -c Release" >> "$TEST_RESULTS_FILE"
    if dotnet pack ../src/Inflop.ConsoleApp.Templates.csproj -c Release 2>&1 | tee -a "$TEST_RESULTS_FILE"; then
        log "✓ Templates packed successfully"
    else
        log_error "Failed to pack templates"
        exit 1
    fi

    log "DEBUG: After pack, continuing to find package file..."

    # Find package file
    log "  - Searching for package file..."
    PACKAGE_FILE=$(find ../src/bin/Release -name "Inflop.ConsoleApp.Templates.*.nupkg" | head -1)
    log "  - Find command completed"

    if [ -z "$PACKAGE_FILE" ]; then
        log_error "Package file not found"
        log_error "Contents of ../src/bin/Release:"
        ls -la ../src/bin/Release/
        ls -la ../src/bin/Release/ >> "$TEST_RESULTS_FILE"
        exit 1
    fi
    log "Package file: $PACKAGE_FILE"

    # Verify package file exists
    if [ ! -f "$PACKAGE_FILE" ]; then
        log_error "Package file found by search but does not exist: $PACKAGE_FILE"
        exit 1
    fi
    log "  - Package file verified: $(ls -lh "$PACKAGE_FILE" | awk '{print $9, $5}')"

    # Step 2: Uninstall previous version
    log "Step 2: Uninstalling previous template version..."
    echo "Executing: dotnet new uninstall Inflop.ConsoleApp.Templates" >> "$TEST_RESULTS_FILE"
    dotnet new uninstall Inflop.ConsoleApp.Templates 2>&1 || true
    echo "✓ Uninstall completed" >> "$TEST_RESULTS_FILE"
    log "✓ Previous version uninstalled (or not installed)"

    # Step 3: Install templates
    log "Step 3: Installing templates..."
    echo "Executing: dotnet new install $PACKAGE_FILE" >> "$TEST_RESULTS_FILE"
    if dotnet new install "$PACKAGE_FILE" 2>&1 | tee -a "$TEST_RESULTS_FILE"; then
        log "✓ Templates installed successfully"
    else
        log_error "Failed to install templates"
        exit 1
    fi

    # Verify installation
    log "Installed templates:"
    dotnet new list | grep inflop
    dotnet new list | grep inflop >> "$TEST_RESULTS_FILE"

    log ""
    log "========================================="
    log "STARTING TEMPLATE TESTS"
    log "========================================="

    # ==========================================
    # INFLOP-SIMPLE TESTS
    # ==========================================

    log ""
    log "========================================="
    log "Testing: inflop-simple"
    log "========================================="

    # Default configuration
    test_template "inflop-simple" "Simple_Default"

    # Framework variations
    test_template "inflop-simple" "Simple_Net80" "-F" "net8.0"
    test_template "inflop-simple" "Simple_Net90" "-F" "net9.0"

    # Async variations
    test_template "inflop-simple" "Simple_Async" "--use-async" "true"
    test_template "inflop-simple" "Simple_Sync" "--use-async" "false"

    # Individual features
    test_template "inflop-simple" "Simple_Serilog" "--add-serilog"
    test_template "inflop-simple" "Simple_Docker" "--add-docker"

    # Command-line parsers
    test_template "inflop-simple" "Simple_SystemCommandLine" "--add-commandline" "system-commandline"
    test_template "inflop-simple" "Simple_SpectreConsole" "--add-commandline" "spectre-console"
    test_template "inflop-simple" "Simple_CommandLineParser" "--add-commandline" "command-line-parser"

    # Database variations
    test_template "inflop-simple" "Simple_Dapper_SQLite" "--add-database" "dapper" "--database-type" "sqlite"
    test_template "inflop-simple" "Simple_Dapper_SqlServer" "--add-database" "dapper" "--database-type" "sqlserver"
    test_template "inflop-simple" "Simple_Dapper_Postgres" "--add-database" "dapper" "--database-type" "postgres"
    test_template "inflop-simple" "Simple_EFCore_SQLite" "--add-database" "efcore" "--database-type" "sqlite"
    test_template "inflop-simple" "Simple_EFCore_SqlServer" "--add-database" "efcore" "--database-type" "sqlserver"
    test_template "inflop-simple" "Simple_EFCore_Postgres" "--add-database" "efcore" "--database-type" "postgres"

    # HTTP Client variations
    test_template "inflop-simple" "Simple_HttpClient_Basic" "--add-httpclient" "basic"
    test_template "inflop-simple" "Simple_HttpClient_Polly" "--add-httpclient" "with-polly"

    # Health Checks variations
    test_template "inflop-simple" "Simple_HealthChecks_Basic" "--add-healthchecks" "basic"
    test_template "inflop-simple" "Simple_HealthChecks_AspNet" "--add-healthchecks" "aspnet"

    # Messaging variations
    test_template "inflop-simple" "Simple_RabbitMQ" "--add-messaging" "rabbitmq"
    test_template "inflop-simple" "Simple_AzureServiceBus" "--add-messaging" "azureservicebus"
    test_template "inflop-simple" "Simple_Kafka" "--add-messaging" "kafka"

    # Complex combinations
    test_template "inflop-simple" "Simple_Full_Net80" \
        "-F" "net8.0" \
        "--use-async" "true" \
        "--add-serilog" \
        "--add-docker" \
        "--add-database" "efcore" \
        "--database-type" "sqlite" \
        "--add-httpclient" "with-polly"

    test_template "inflop-simple" "Simple_Full_Net90" \
        "-F" "net9.0" \
        "--use-async" "true" \
        "--add-serilog" \
        "--add-docker" \
        "--add-database" "efcore" \
        "--database-type" "postgres" \
        "--add-httpclient" "with-polly" \
        "--add-healthchecks" "aspnet" \
        "--add-messaging" "rabbitmq"

    # ==========================================
    # INFLOP-STANDARD TESTS
    # ==========================================

    log ""
    log "========================================="
    log "Testing: inflop-standard"
    log "========================================="

    # Default configuration
    test_template "inflop-standard" "Standard_Default"

    # Framework variations
    test_template "inflop-standard" "Standard_Net80" "-F" "net8.0"
    test_template "inflop-standard" "Standard_Net90" "-F" "net9.0"

    # Async variations
    test_template "inflop-standard" "Standard_Async" "--use-async" "true"
    test_template "inflop-standard" "Standard_Sync" "--use-async" "false"

    # Individual features
    test_template "inflop-standard" "Standard_Serilog" "--add-serilog"
    test_template "inflop-standard" "Standard_Docker" "--add-docker"

    # Database variations
    test_template "inflop-standard" "Standard_Dapper_SQLite" "--add-database" "dapper" "--database-type" "sqlite"
    test_template "inflop-standard" "Standard_EFCore_Postgres" "--add-database" "efcore" "--database-type" "postgres"

    # HTTP Client variations
    test_template "inflop-standard" "Standard_HttpClient_Polly" "--add-httpclient" "with-polly"

    # Health Checks variations
    test_template "inflop-standard" "Standard_HealthChecks_AspNet" "--add-healthchecks" "aspnet"

    # Messaging variations
    test_template "inflop-standard" "Standard_RabbitMQ" "--add-messaging" "rabbitmq"

    # Complex combination
    test_template "inflop-standard" "Standard_Full" \
        "-F" "net9.0" \
        "--use-async" "true" \
        "--add-serilog" \
        "--add-docker" \
        "--add-database" "efcore" \
        "--database-type" "sqlserver" \
        "--add-httpclient" "with-polly" \
        "--add-healthchecks" "aspnet" \
        "--add-messaging" "azureservicebus"

    # ==========================================
    # INFLOP-ADVANCED TESTS
    # ==========================================

    log ""
    log "========================================="
    log "Testing: inflop-advanced"
    log "========================================="

    # Default configuration
    test_template "inflop-advanced" "Advanced_Default"

    # Framework variations
    test_template "inflop-advanced" "Advanced_Net80" "-F" "net8.0"
    test_template "inflop-advanced" "Advanced_Net90" "-F" "net9.0"

    # Async variations
    test_template "inflop-advanced" "Advanced_Async" "--use-async" "true"
    test_template "inflop-advanced" "Advanced_Sync" "--use-async" "false"

    # Individual features
    test_template "inflop-advanced" "Advanced_Serilog" "--add-serilog"
    test_template "inflop-advanced" "Advanced_Docker" "--add-docker"

    # Database variations
    test_template "inflop-advanced" "Advanced_Dapper_SQLite" "--add-database" "dapper" "--database-type" "sqlite"
    test_template "inflop-advanced" "Advanced_EFCore_Postgres" "--add-database" "efcore" "--database-type" "postgres"

    # HTTP Client variations
    test_template "inflop-advanced" "Advanced_HttpClient_Polly" "--add-httpclient" "with-polly"

    # Health Checks variations
    test_template "inflop-advanced" "Advanced_HealthChecks_AspNet" "--add-healthchecks" "aspnet"

    # Messaging variations
    test_template "inflop-advanced" "Advanced_Kafka" "--add-messaging" "kafka"

    # Complex combination
    test_template "inflop-advanced" "Advanced_Full" \
        "-F" "net9.0" \
        "--use-async" "true" \
        "--add-serilog" \
        "--add-docker" \
        "--add-database" "efcore" \
        "--database-type" "postgres" \
        "--add-httpclient" "with-polly" \
        "--add-healthchecks" "aspnet" \
        "--add-messaging" "kafka"

    # ==========================================
    # INFLOP-ENTERPRISE TESTS
    # ==========================================

    log ""
    log "========================================="
    log "Testing: inflop-enterprise"
    log "========================================="

    # Default configuration
    test_template "inflop-enterprise" "Enterprise_Default"

    # Framework variations
    test_template "inflop-enterprise" "Enterprise_Net80" "-F" "net8.0"
    test_template "inflop-enterprise" "Enterprise_Net90" "-F" "net9.0"

    # Async variations
    test_template "inflop-enterprise" "Enterprise_Async" "--use-async" "true"
    test_template "inflop-enterprise" "Enterprise_Sync" "--use-async" "false"

    # Individual features
    test_template "inflop-enterprise" "Enterprise_Serilog" "--add-serilog"
    test_template "inflop-enterprise" "Enterprise_Docker" "--add-docker"

    # Database variations
    test_template "inflop-enterprise" "Enterprise_Dapper_SQLite" "--add-database" "dapper" "--database-type" "sqlite"
    test_template "inflop-enterprise" "Enterprise_EFCore_SqlServer" "--add-database" "efcore" "--database-type" "sqlserver"

    # HTTP Client variations
    test_template "inflop-enterprise" "Enterprise_HttpClient_Polly" "--add-httpclient" "with-polly"

    # Health Checks variations
    test_template "inflop-enterprise" "Enterprise_HealthChecks_AspNet" "--add-healthchecks" "aspnet"

    # Messaging variations
    test_template "inflop-enterprise" "Enterprise_AzureServiceBus" "--add-messaging" "azureservicebus"

    # Complex combination
    test_template "inflop-enterprise" "Enterprise_Full" \
        "-F" "net9.0" \
        "--use-async" "true" \
        "--add-serilog" \
        "--add-docker" \
        "--add-commandline" "spectre-console" \
        "--add-database" "efcore" \
        "--database-type" "sqlserver" \
        "--add-httpclient" "with-polly" \
        "--add-healthchecks" "aspnet" \
        "--add-messaging" "azureservicebus"

    # ==========================================
    # FINAL SUMMARY
    # ==========================================

    log ""
    log "========================================="
    log "TEST SUMMARY"
    log "========================================="
    log "Total tests:  $TOTAL_TESTS"
    echo -e "${GREEN}[✓ PASS]${NC} Passed:       $PASSED_TESTS" >&2
    echo "[✓ PASS] Passed:       $PASSED_TESTS" >> "$TEST_RESULTS_FILE"
    echo -e "${RED}[✗ FAIL]${NC} Failed:       $FAILED_TESTS" >&2
    echo "[✗ FAIL] Failed:       $FAILED_TESTS" >> "$TEST_RESULTS_FILE"
    log "Success rate: $((PASSED_TESTS * 100 / TOTAL_TESTS))%"
    log "========================================="
    log "Completed: $(date)"
    log "Test results saved to: $TEST_RESULTS_FILE"
    log "Test directory: $TEST_DIR"
    log "========================================="

    # Cleanup
    if [ $FAILED_TESTS -eq 0 ]; then
        log ""
        log "All tests passed - cleaning up test directory..."
        rm -rf "$TEST_DIR"
        log "✓ Cleanup complete"
    else
        log ""
        log "⚠ Some tests failed - keeping test directory for investigation:"
        log "  $TEST_DIR"
    fi

    # Return exit code based on failures
    if [ $FAILED_TESTS -gt 0 ]; then
        exit 1
    fi

    exit 0
}

# Run main function
main "$@"
