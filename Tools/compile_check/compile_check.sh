#!/usr/bin/env bash
# Offline compile verification of all game code against Unity API stubs.
set -e
cd "$(dirname "$0")"
# Common local SDK locations (sandbox installs it in the home dir, not on PATH).
for d in /home/user/build/dotnet /home/user/dotnet /home/user/.dotnet "$HOME/dotnet" "$HOME/.dotnet"; do
    if [ -x "$d/dotnet" ]; then
        export PATH="$d:$PATH"
        export DOTNET_ROOT="$d"
        break
    fi
done
if ! command -v dotnet >/dev/null 2>&1; then
    echo "dotnet SDK not found - install .NET SDK 8 first (https://dotnet.microsoft.com)."
    exit 1
fi
dotnet build compile_check.csproj -v q
echo "COMPILE CHECK PASSED"
