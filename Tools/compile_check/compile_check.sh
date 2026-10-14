#!/usr/bin/env bash
# Offline compile verification of all game code against Unity API stubs.
set -e
cd "$(dirname "$0")"
if ! command -v dotnet >/dev/null 2>&1; then
    echo "dotnet SDK not found - install .NET SDK 8 first (https://dotnet.microsoft.com)."
    exit 1
fi
dotnet build compile_check.csproj -v q
echo "COMPILE CHECK PASSED"
