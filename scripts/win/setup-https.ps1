# scripts/win/setup-https.ps1

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$projectRoot = Split-Path -Parent $scriptDir
$certDir = Join-Path $projectRoot "scripts\certs"
$certPath = Join-Path $certDir "lha-dev.pfx"
$certPassword = "cryptic_password_LHA_2026"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "   LHA HTTPS Certificate Setup & Apply for Windows" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

if (-not (Test-Path $certDir)) {
    New-Item -ItemType Directory -Path $certDir -Force | Out-Null
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "Error: dotnet command not found. Please install .NET SDK." -ForegroundColor Red
    exit 1
}

# 1. Generate/Renew Certificate
if (-not (Test-Path $certPath)) {
    Write-Host "--> Generating new dev certificate..."
    dotnet dev-certs https --clean
    dotnet dev-certs https --trust
    dotnet dev-certs https --export-path $certPath --password $certPassword
} else {
    Write-Host "--> Dev certificate already exists at $certPath"
}

# 2. Export environment variables for the current session
Write-Host "--> Setting environment variables..."
$env:ASPNETCORE_Kestrel__Certificates__Default__Path = $certPath
$env:ASPNETCORE_Kestrel__Certificates__Default__Password = $certPassword

# 3. Create a helper file to source (PowerShell)
$ps1EnvPath = Join-Path $projectRoot "scripts\.env.https.ps1"
@"
`$env:ASPNETCORE_Kestrel__Certificates__Default__Path = "$certPath"
`$env:ASPNETCORE_Kestrel__Certificates__Default__Password = "$certPassword"
"@ | Set-Content -Path $ps1EnvPath -Encoding UTF8

Write-Host "--------------------------------------------------------" -ForegroundColor Green
Write-Host "SUCCESS: Certificate is ready." -ForegroundColor Green
Write-Host "Path: $certPath" -ForegroundColor Green
Write-Host "--------------------------------------------------------" -ForegroundColor Cyan
Write-Host ""
Write-Host "To apply these settings to your current terminal, run:"
Write-Host "    . ./scripts/.env.https.ps1"
Write-Host ""
Write-Host "After that, you can run any project (e.g., dotnet run) and it will use this HTTPS certificate."
Write-Host ""
Write-Host "==========================================================" -ForegroundColor Cyan
