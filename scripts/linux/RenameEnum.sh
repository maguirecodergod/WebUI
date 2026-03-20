#!/bin/bash

basePath="/workspaces/WebUI/src"

enums=(
    "EntityChangeType"
)

echo "Updating references in files..."

# 1. Update content in files
while IFS= read -r -d '' file; do
    changed=false
    content="$(cat "$file")"

    for enum in "${enums[@]}"; do
        oldName="$enum"
        newName="C$enum"

        if echo "$content" | grep -q -w "$oldName"; then
            echo "Replacing $oldName -> $newName in $file"
            content="$(echo "$content" | sed -E "s/\b${oldName}\b/${newName}/g")"
            changed=true
        fi
    done

    if [ "$changed" = true ]; then
        echo "$content" > "$file"
    fi
done < <(find "$basePath" -type f \( -name "*.cs" -o -name "*.razor" \) -print0)

# 2. Rename enum files
echo "Renaming enum files..."

for enum in "${enums[@]}"; do
    oldName="$enum"
    newName="C$enum"

    while IFS= read -r -d '' file; do
        dir="$(dirname "$file")"
        newPath="$dir/$newName.cs"

        echo "Renaming file $file -> $newPath"
        mv "$file" "$newPath"
    done < <(find "$basePath" -type f -name "$oldName.cs" -print0)
done

echo "Enums renamed successfully according to convention!"