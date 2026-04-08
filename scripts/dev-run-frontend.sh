#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/../frontend/library-app"

npm install
npm start

