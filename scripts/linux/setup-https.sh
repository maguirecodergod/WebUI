#!/bin/bash
# scripts/linux/setup-https.sh

# Get absolute path of this script's directory
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
PROJECT_ROOT="$(dirname "$(dirname "$DIR")")"
CERT_DIR="$PROJECT_ROOT/scripts/certs"
CERT_PATH="$CERT_DIR/lha-dev.pfx"
CERT_PASSWORD="cryptic_password_LHA_2026"

echo "=========================================================="
echo "   LHA HTTPS Certificate Setup & Apply for Linux"
echo "=========================================================="

mkdir -p "$CERT_DIR"

if ! command -v dotnet &> /dev/null
then
    echo "Error: dotnet command not found. Please install .NET SDK."
    exit 1
fi

# 1. Generate/Renew Certificate
if [ ! -f "$CERT_PATH" ]; then
    echo "--> Generating new dev certificate..."
    dotnet dev-certs https --clean
    dotnet dev-certs https --export-path "$CERT_PATH" --password "$CERT_PASSWORD"
else
    echo "--> Dev certificate already exists at $CERT_PATH"
fi

# 2. Export environment variables for the current session
# Note: To apply to the calling shell, the user must run "source ./scripts/linux/setup-https.sh"
echo "--> Setting environment variables..."
export ASPNETCORE_Kestrel__Certificates__Default__Path="$CERT_PATH"
export ASPNETCORE_Kestrel__Certificates__Default__Password="$CERT_PASSWORD"

# 3. Create a helper file to source
cat <<EOF > "$PROJECT_ROOT/scripts/.env.https"
export ASPNETCORE_Kestrel__Certificates__Default__Path="$CERT_PATH"
export ASPNETCORE_Kestrel__Certificates__Default__Password="$CERT_PASSWORD"
EOF

echo "----------------------------------------------------------"
echo "SUCCESS: Certificate is ready."
echo "Path: $CERT_PATH"
echo "----------------------------------------------------------"
echo ""
echo "To apply these settings to your current terminal, run:"
echo "    source ./scripts/.env.https"
echo ""
echo "After that, you can run any project (e.g., dotnet run) and it will use this HTTPS certificate."
echo ""
echo "Tip for Codespaces: You can add 'source $(pwd)/scripts/.env.https' to your ~/.bashrc"
echo "=========================================================="
