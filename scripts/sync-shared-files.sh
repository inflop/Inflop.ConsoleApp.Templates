#!/bin/bash
# sync-shared-files.sh
# Synchronizes shared files from src/_shared/ to all template directories

set -e

SHARED_DIR="../src/_shared"
TEMPLATES=(
    "../src/templates/1-ConsoleApp.Simple"
    "../src/templates/2-ConsoleApp.Standard"
    "../src/templates/3-ConsoleApp.Advanced"
    "../src/templates/4-ConsoleApp.Enterprise"
)

# Function to determine target namespace from template path
get_target_namespace() {
    local template_path="$1"
    case "$template_path" in
        *"1-ConsoleApp.Simple"*)
            echo "ConsoleApp.Simple"
            ;;
        *"2-ConsoleApp.Standard"*)
            echo "ConsoleApp.Standard"
            ;;
        *"3-ConsoleApp.Advanced"*)
            echo "ConsoleApp.Advanced"
            ;;
        *"4-ConsoleApp.Enterprise"*)
            echo "ConsoleApp.Enterprise"
            ;;
        *)
            echo "ConsoleApp.Advanced" # fallback
            ;;
    esac
}

# Function to copy file and replace namespace
copy_and_replace_namespace() {
    local source_file="$1"
    local dest_file="$2"
    local target_namespace="$3"

    # Copy file and replace ConsoleApp.Advanced with target namespace
    sed "s/ConsoleApp\.Advanced/$target_namespace/g" "$source_file" > "$dest_file"
}

echo "========================================="
echo "Syncing shared files to all templates..."
echo "========================================="
echo ""

for template in "${TEMPLATES[@]}"; do
    echo "📦 Syncing to: $template"

    # Determine target namespace for this template
    TARGET_NS=$(get_target_namespace "$template")
    echo "  🎯 Target namespace: $TARGET_NS"

    # Template config
    if [ -f "$SHARED_DIR/.template.config/dotnetcli.host.json" ]; then
        mkdir -p "$template/.template.config"
        cp "$SHARED_DIR/.template.config/dotnetcli.host.json" "$template/.template.config/" 2>/dev/null || true
        echo "  ✓ Template config synced"
    fi

    # Extensions (skip for Simple template - it uses inline DI)
    if [ -d "$SHARED_DIR/Extensions" ] && [ "$template" != "../src/templates/1-ConsoleApp.Simple" ]; then
        mkdir -p "$template/Extensions"
        for file in "$SHARED_DIR/Extensions/"*.cs; do
            [ -f "$file" ] || continue
            filename=$(basename "$file")
            copy_and_replace_namespace "$file" "$template/Extensions/$filename" "$TARGET_NS"
        done
        echo "  ✓ Extensions synced (namespace replaced)"
    fi

    # Infrastructure
    if [ -d "$SHARED_DIR/Infrastructure" ]; then
        mkdir -p "$template/Infrastructure"
        for file in "$SHARED_DIR/Infrastructure/"*.cs; do
            [ -f "$file" ] || continue
            filename=$(basename "$file")
            copy_and_replace_namespace "$file" "$template/Infrastructure/$filename" "$TARGET_NS"
        done
        echo "  ✓ Infrastructure synced (namespace replaced)"
    fi

    # Services (only API client files)
    if [ -d "$SHARED_DIR/Services" ]; then
        mkdir -p "$template/Services"
        [ -f "$SHARED_DIR/Services/IApiClient.cs" ] && copy_and_replace_namespace "$SHARED_DIR/Services/IApiClient.cs" "$template/Services/IApiClient.cs" "$TARGET_NS"
        [ -f "$SHARED_DIR/Services/ApiClient.cs" ] && copy_and_replace_namespace "$SHARED_DIR/Services/ApiClient.cs" "$template/Services/ApiClient.cs" "$TARGET_NS"
        echo "  ✓ Services synced (namespace replaced)"
    fi

    # Messaging
    if [ -d "$SHARED_DIR/Messaging" ]; then
        mkdir -p "$template/Messaging"
        for file in "$SHARED_DIR/Messaging/"*.cs; do
            [ -f "$file" ] || continue
            filename=$(basename "$file")
            copy_and_replace_namespace "$file" "$template/Messaging/$filename" "$TARGET_NS"
        done
        echo "  ✓ Messaging synced (namespace replaced)"
    fi

    # Health
    if [ -d "$SHARED_DIR/Health" ]; then
        mkdir -p "$template/Health"
        for file in "$SHARED_DIR/Health/"*.cs; do
            [ -f "$file" ] || continue
            filename=$(basename "$file")
            copy_and_replace_namespace "$file" "$template/Health/$filename" "$TARGET_NS"
        done
        echo "  ✓ Health synced (namespace replaced)"
    fi

    # Data
    if [ -d "$SHARED_DIR/Data" ]; then
        mkdir -p "$template/Data/Models"
        for file in "$SHARED_DIR/Data/"*.cs; do
            [ -f "$file" ] || continue
            filename=$(basename "$file")
            copy_and_replace_namespace "$file" "$template/Data/$filename" "$TARGET_NS"
        done
        if [ -d "$SHARED_DIR/Data/Models" ]; then
            for file in "$SHARED_DIR/Data/Models/"*.cs; do
                [ -f "$file" ] || continue
                filename=$(basename "$file")
                copy_and_replace_namespace "$file" "$template/Data/Models/$filename" "$TARGET_NS"
            done
        fi
        echo "  ✓ Data synced (namespace replaced)"
    fi

    echo ""
done

echo "========================================="
echo "✅ Sync complete!"
echo "========================================="
echo ""
echo "Files synced from $SHARED_DIR to:"
for template in "${TEMPLATES[@]}"; do
    echo "  - $template"
done
echo ""
