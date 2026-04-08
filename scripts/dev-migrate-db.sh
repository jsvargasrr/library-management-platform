#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."

dotnet tool restore --tool-manifest backend/.config/dotnet-tools.json
dotnet tool run --tool-manifest backend/.config/dotnet-tools.json dotnet-ef database update \
  -p backend/src/Library.Infrastructure/Library.Infrastructure.csproj \
  -s backend/src/Library.Api/Library.Api.csproj

echo "Migraciones aplicadas."

