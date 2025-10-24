param()
$ErrorActionPreference = 'Stop'
Write-Host '==> Building solution'
dotnet build -c Release
Write-Host '==> Running tests'
dotnet test -c Release --no-build --collect:"XPlat Code Coverage"
Write-Host '==> README validation (placeholder)'
# Placeholder for future readme validation command
Write-Host 'Verification complete.'