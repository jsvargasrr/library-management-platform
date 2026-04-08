#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."

docker compose -f database/docker-compose.yml up -d

echo "SQL Server levantado en localhost:14333"

