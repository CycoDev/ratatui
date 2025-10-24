#!/usr/bin/env bash
set -euo pipefail

echo "==> Building solution"
dotnet build -c Release

echo "==> Running tests"
dotnet test -c Release --no-build --collect:"XPlat Code Coverage"

echo "==> README validation (placeholder)"
# Placeholder: would invoke readme tool: dotnet run --project tools/Docs -- readme --check || true

echo "Verification complete."