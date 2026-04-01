#!/bin/bash
# scripts/linux/setup-https.sh

# Get absolute path of this script's directory
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
PROJECT_ROOT="$(dirname "$(dirname "$DIR")")"
CERT_DIR="$PROJECT_ROOT/scripts/certs"
CERT_PATH="$CERT_DIR/lha-dev.pfx"
CERT_CRT_PATH="$CERT_DIR/lha-dev.crt"
CERT_PASSWORD="cryptic_password_LHA_2026"

echo "=========================================================="
echo "   LHA HTTPS Certificate Setup & Trust for Linux"
echo "=========================================================="

mkdir -p "$CERT_DIR"

if ! command -v dotnet &> /dev/null
then
    echo "Error: dotnet command not found. Please install .NET SDK."
    exit 1
fi

# 1. Generate/Renew Certificate
if [ ! -f "$CERT_PATH" ]; then
    echo "--> Generating and exporting new dev certificate..."
    dotnet dev-certs https --clean
    dotnet dev-certs https --export-path "$CERT_PATH" --password "$CERT_PASSWORD"
else
    echo "--> Dev certificate already exists at $CERT_PATH"
fi

# 2. Extract public certificate for Linux Trust Store (.crt)
echo "--> Extracting public certificate to PEM format..."
openssl pkcs12 -in "$CERT_PATH" -clcerts -nokeys -out "$CERT_CRT_PATH" -password "pass:$CERT_PASSWORD"

# 3. Trust the certificate (requires sudo)
if [ -d "/usr/local/share/ca-certificates" ]; then
    echo "--> Adding certificate to System Trust Store (requires sudo)..."
    sudo cp "$CERT_CRT_PATH" /usr/local/share/ca-certificates/lha-dev.crt
    sudo update-ca-certificates
    echo "--> Certificate trusted by OS."
else
    echo "Warning: /usr/local/share/ca-certificates not found. Skipping OS trust."
fi

# 4. Export environment variables
echo "--> Setting environment variables..."
export ASPNETCORE_Kestrel__Certificates__Default__Path="$CERT_PATH"
export ASPNETCORE_Kestrel__Certificates__Default__Password="$CERT_PASSWORD"

# 5. Create a helper file to source
cat <<EOF > "$PROJECT_ROOT/scripts/.env.https"
export ASPNETCORE_Kestrel__Certificates__Default__Path="$CERT_PATH"
export ASPNETCORE_Kestrel__Certificates__Default__Password="$CERT_PASSWORD"
EOF

echo "----------------------------------------------------------"
echo "SUCCESS: Certificate is ready and trusted."
echo "Path: $CERT_PATH"
echo "----------------------------------------------------------"
echo ""
echo "To apply these settings to your current terminal, run:"
echo "    source ./scripts/.env.https"
echo ""
echo "After that, you can run any project (e.g., dotnet run) and it will work!"
echo "=========================================================="
