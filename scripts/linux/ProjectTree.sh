#!/bin/bash

# Define the output file
OUTPUT_FILE="scripts/ProjectStructure.md"

# Write the header to the output file
echo "# 📂 Project Structure" > "$OUTPUT_FILE"
echo "" >> "$OUTPUT_FILE"
echo "_Generated automatically on $(date '+%a %b %d %r %Z %Y')_" >> "$OUTPUT_FILE"
echo "" >> "$OUTPUT_FILE"

# Start the code block
echo "\`\`\`" >> "$OUTPUT_FILE"

# Generate the project structure tree
# The 'tree' command is used to display the directory structure.
# It reads patterns from .gitignore and excludes them from the tree.
if [ -f .gitignore ]; then
    ignore_args=()
    while IFS= read -r pattern; do
        # skip comments and empty lines
        if [[ "$pattern" != "" && "$pattern" != "#"* ]]; then
            ignore_args+=("-I" "$pattern")
        fi
    done < .gitignore
    
    # Also ignore .git directory and the script itself
    ignore_args+=("-I" ".git")
    ignore_args+=("-I" "projectstructuretree.sh")

    tree "${ignore_args[@]}" >> "$OUTPUT_FILE"
else
    tree -I ".git|projectstructuretree.sh" >> "$OUTPUT_FILE"
fi

# End the code block
echo "\`\`\`" >> "$OUTPUT_FILE"