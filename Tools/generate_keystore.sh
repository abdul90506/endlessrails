#!/usr/bin/env bash
# =============================================================================
# Endless Rails - development keystore generator
# =============================================================================
# Creates a long-lived self-signed keystore used to sign release APKs/AABs.
# For a published game, REPLACE this with your own production keystore and
# keep it (and its passwords) out of version control.
# =============================================================================
set -euo pipefail

DIR="$(cd "$(dirname "$0")" && pwd)/keystore"
STORE="$DIR/endlessrails-release.keystore"
ALIAS="endlessrails"
PASS="endlessrails2026"

mkdir -p "$DIR"

if [[ -f "$STORE" ]]; then
    echo "Keystore already exists: $STORE"
    exit 0
fi

# Prefer keytool from PATH, then common JDK locations.
KEYTOOL="$(command -v keytool || true)"
if [[ -z "$KEYTOOL" ]]; then
    for candidate in /usr/lib/jvm/*/bin/keytool "$JAVA_HOME/bin/keytool"; do
        if [[ -x "$candidate" ]]; then
            KEYTOOL="$candidate"
            break
        fi
    done
fi

if [[ -z "$KEYTOOL" ]]; then
    echo "ERROR: keytool not found. Install a JDK (8+) and re-run."
    exit 1
fi

"$KEYTOOL" -genkeypair -v \
    -keystore "$STORE" \
    -alias "$ALIAS" \
    -keyalg RSA -keysize 2048 \
    -validity 10000 \
    -storepass "$PASS" \
    -keypass "$PASS" \
    -dname "CN=Endless Rails Development, OU=Games, O=EndlessRailsStudio, L=Islamabad, S=Islamabad, C=PK"

echo ""
echo "Keystore created: $STORE"
echo "  alias:     $ALIAS"
echo "  storepass: $PASS"
echo "  keypass:   $PASS"
echo ""
echo "NOTE: these credentials are for DEVELOPMENT ONLY."
echo "Use your own keystore before publishing to the Play Store."
