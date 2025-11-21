#!/bin/bash

# Debug script to identify where test-all-templates.sh stops

set -x  # Enable debug mode - shows each command as it executes

echo "=== DEBUG TEST SCRIPT ==="
echo "Working directory: $(pwd)"
echo ""

# Step 1: Pack templates
echo "Step 1: Packing templates..."
cd "$(dirname "$0")"
echo "Changed to: $(pwd)"

rm -rf ../src/bin/Release/*.nupkg 2>/dev/null || true

if ! dotnet pack ../src/Inflop.ConsoleApp.Templates.csproj -c Release; then
    echo "ERROR: Pack failed"
    exit 1
fi
echo "Pack succeeded"
echo ""

# Step 2: Find package file
echo "Step 2: Finding package file..."
PACKAGE_FILE=$(find ../src/bin/Release -name "Inflop.ConsoleApp.Templates.*.nupkg" | head -1)

echo "Package file search result: '$PACKAGE_FILE'"

if [ -z "$PACKAGE_FILE" ]; then
    echo "ERROR: Package file not found"
    echo "Contents of ../src/bin/Release:"
    ls -la ../src/bin/Release/ || echo "Directory doesn't exist"
    exit 1
fi

echo "Package file found: $PACKAGE_FILE"
echo "Package file exists: $(test -f "$PACKAGE_FILE" && echo "YES" || echo "NO")"
echo ""

# Step 3: Uninstall previous version
echo "Step 3: Uninstalling previous templates..."
dotnet new uninstall Inflop.ConsoleApp.Templates 2>&1 || echo "No previous installation"
echo ""

# Step 4: Install templates
echo "Step 4: Installing templates..."
echo "Installing from: $PACKAGE_FILE"

if ! dotnet new install "$PACKAGE_FILE"; then
    echo "ERROR: Install failed"
    exit 1
fi
echo "Install succeeded"
echo ""

# Step 5: Verify installation
echo "Step 5: Verifying installation..."
dotnet new list | grep inflop || echo "Templates not found in list"
echo ""

echo "=== ALL STEPS COMPLETED ==="
