#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."

dotnet run --project backend/src/Library.Api/Library.Api.csproj

