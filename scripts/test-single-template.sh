#!/bin/bash

# Quick Single Template Test Script
# Usage: ./test-single-template.sh <template-name> <project-name> [additional-params...]
#
# Examples:
#   ./test-single-template.sh inflop-simple MyTest
#   ./test-single-template.sh inflop-simple MyTest --add-serilog --add-docker
#   ./test-single-template.sh inflop-enterprise MyApp -F net9.0 --add-database efcore

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Check arguments
if [ $# -lt 2 ]; then
    echo -e "${RED}Error: Missing arguments${NC}"
    echo ""
    echo "Usage: $0 <template-name> <project-name> [additional-params...]"
    echo ""
    echo "Templates available:"
    echo "  inflop-simple      - Simple DI console app"
    echo "  inflop-standard    - Standard Generic Host app"
    echo "  inflop-advanced    - Advanced with extension methods"
    echo "  inflop-enterprise  - Enterprise with strongly-typed config"
    echo ""
    echo "Examples:"
    echo "  $0 inflop-simple MyTest"
    echo "  $0 inflop-simple MyTest --add-serilog --add-docker"
    echo "  $0 inflop-enterprise MyApp -F net9.0 --add-database efcore"
    echo ""
    exit 1
fi

TEMPLATE_NAME=$1
PROJECT_NAME=$2
shift 2
PARAMS=("$@")

TEST_DIR="/tmp/inflop-test-$(date +%s)"

echo -e "${BLUE}=========================================${NC}"
echo -e "${BLUE}Single Template Test${NC}"
echo -e "${BLUE}=========================================${NC}"
echo -e "Template:    ${GREEN}$TEMPLATE_NAME${NC}"
echo -e "Project:     ${GREEN}$PROJECT_NAME${NC}"
echo -e "Parameters:  ${GREEN}${PARAMS[*]}${NC}"
echo -e "Test dir:    ${GREEN}$TEST_DIR${NC}"
echo -e "${BLUE}=========================================${NC}"
echo ""

# Create test directory
mkdir -p "$TEST_DIR"

# Step 1: Pack templates
echo -e "${YELLOW}[1/6]${NC} Packing templates..."
cd "$(dirname "$0")"
rm -rf ../src/bin/Release/*.nupkg 2>/dev/null || true
if ! dotnet pack ../src/Inflop.ConsoleApp.Templates.csproj -c Release > /dev/null 2>&1; then
    echo -e "${RED}✗ Failed to pack templates${NC}"
    echo "Running pack with output:"
    dotnet pack ../src/Inflop.ConsoleApp.Templates.csproj -c Release
    exit 1
fi
echo -e "${GREEN}✓ Templates packed${NC}"

# Find package file
PACKAGE_FILE=$(find ../src/bin/Release -name "Inflop.ConsoleApp.Templates.*.nupkg" | head -1)
if [ -z "$PACKAGE_FILE" ]; then
    echo -e "${RED}✗ Package file not found${NC}"
    exit 1
fi

# Step 2: Uninstall previous version
echo -e "${YELLOW}[2/6]${NC} Uninstalling previous version..."
dotnet new uninstall Inflop.ConsoleApp.Templates > /dev/null 2>&1 || true
echo -e "${GREEN}✓ Previous version uninstalled${NC}"

# Step 3: Install templates
echo -e "${YELLOW}[3/6]${NC} Installing templates..."
if ! dotnet new install "$PACKAGE_FILE" > /dev/null 2>&1; then
    echo -e "${RED}✗ Failed to install templates${NC}"
    exit 1
fi
echo -e "${GREEN}✓ Templates installed${NC}"

# Step 4: Generate project
echo -e "${YELLOW}[4/6]${NC} Generating project from template..."
cd "$TEST_DIR"
echo -e "${BLUE}Command:${NC} dotnet new $TEMPLATE_NAME -n $PROJECT_NAME ${PARAMS[*]}"
if ! dotnet new "$TEMPLATE_NAME" -n "$PROJECT_NAME" "${PARAMS[@]}"; then
    echo -e "${RED}✗ Template generation failed${NC}"
    exit 1
fi
echo -e "${GREEN}✓ Project generated${NC}"

# Step 5: Build project
echo -e "${YELLOW}[5/6]${NC} Building project..."
cd "$PROJECT_NAME"

echo "  - Running restore..."
if ! dotnet restore > /dev/null 2>&1; then
    echo -e "${RED}✗ Restore failed${NC}"
    dotnet restore
    exit 1
fi
echo -e "  ${GREEN}✓ Restore successful${NC}"

echo "  - Running build..."
if ! dotnet build > /dev/null 2>&1; then
    echo -e "${RED}✗ Build failed${NC}"
    dotnet build
    exit 1
fi
echo -e "  ${GREEN}✓ Build successful${NC}"

# Step 6: Verify generated code
echo -e "${YELLOW}[6/6]${NC} Verifying generated code..."

# Check for unprocessed template directives
if grep -r "//#if\|//#endif" --include="*.cs" . > /dev/null 2>&1; then
    echo -e "${YELLOW}⚠ Warning: Found unprocessed template directives${NC}"
    grep -r "//#if\|//#endif" --include="*.cs" .
else
    echo -e "  ${GREEN}✓ No template directives found${NC}"
fi

# Verify target framework
FRAMEWORK=$(grep -oP '(?<=<TargetFramework>)[^<]+' *.csproj | head -1)
echo -e "  ${GREEN}✓ Target framework: $FRAMEWORK${NC}"

# Show project structure
echo ""
echo -e "${BLUE}Project structure:${NC}"
tree -L 2 -I 'bin|obj' . || find . -maxdepth 2 -not -path '*/bin/*' -not -path '*/obj/*' | head -20

echo ""
echo -e "${BLUE}=========================================${NC}"
echo -e "${GREEN}✓ ALL CHECKS PASSED${NC}"
echo -e "${BLUE}=========================================${NC}"
echo ""
echo -e "Project location: ${GREEN}$TEST_DIR/$PROJECT_NAME${NC}"
echo ""
echo "Next steps:"
echo "  cd $TEST_DIR/$PROJECT_NAME"
echo "  dotnet run"
echo ""
echo "To clean up test files:"
echo "  rm -rf $TEST_DIR"
echo ""
