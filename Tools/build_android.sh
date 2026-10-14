#!/usr/bin/env bash
# =============================================================================
# Endless Rails - Android build script (APK + AAB)
# =============================================================================
# Builds a signed release APK and AAB using Unity in batch mode.
#
# Usage:
#   UNITY_PATH=/path/to/Unity ./Tools/build_android.sh
#
# Defaults to the Unity Hub install path for Unity 6000.0 LTS.
# Requires: Unity 6 (6000.0.x) with Android Build Support + IL2CPP module.
# =============================================================================
set -euo pipefail

PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
UNITY_VERSION="6000.0.32f1"

# --- Locate Unity ---
if [[ -n "${UNITY_PATH:-}" ]]; then
    UNITY="$UNITY_PATH"
elif [[ -x "/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity" ]]; then
    UNITY="/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity"
elif [[ -x "$HOME/Unity/Hub/Editor/${UNITY_VERSION}/Editor/Unity" ]]; then
    UNITY="$HOME/Unity/Hub/Editor/${UNITY_VERSION}/Editor/Unity"
elif [[ -x "/opt/unity/editor/Unity" ]]; then
    UNITY="/opt/unity/editor/Unity"          # game-ci docker image
else
    echo "ERROR: Unity ${UNITY_VERSION} not found. Set UNITY_PATH to your Unity editor binary."
    echo "  Example: UNITY_PATH=\"\$HOME/Unity/Hub/Editor/${UNITY_VERSION}/Editor/Unity\" $0"
    exit 1
fi

echo "==> Using Unity: $UNITY"
echo "==> Project:     $PROJECT_PATH"

# --- Generate the development keystore if missing ---
if [[ ! -f "$PROJECT_PATH/Tools/keystore/endlessrails-release.keystore" ]]; then
    echo "==> Generating development keystore..."
    bash "$PROJECT_PATH/Tools/generate_keystore.sh"
fi

# --- Project setup + asset generation (idempotent) ---
echo "==> Running project setup & asset generation..."
"$UNITY" -batchmode -quit -nographics \
    -projectPath "$PROJECT_PATH" \
    -executeMethod EndlessRails.EditorTools.ProjectSetup.SetupProject \
    -logFile "$PROJECT_PATH/Builds/setup.log"

"$UNITY" -batchmode -quit -nographics \
    -projectPath "$PROJECT_PATH" \
    -executeMethod EndlessRails.EditorTools.AssetFactory.GenerateAll \
    -logFile "$PROJECT_PATH/Builds/assets.log"

"$UNITY" -batchmode -quit -nographics \
    -projectPath "$PROJECT_PATH" \
    -executeMethod EndlessRails.EditorTools.SceneBuilder.BuildScene \
    -logFile "$PROJECT_PATH/Builds/scene.log"

# --- Build APK ---
echo "==> Building signed release APK..."
"$UNITY" -batchmode -quit -nographics \
    -projectPath "$PROJECT_PATH" \
    -executeMethod EndlessRails.EditorTools.BuildScript.BuildAndroidApk \
    -logFile "$PROJECT_PATH/Builds/apk.log"

# --- Build AAB ---
echo "==> Building signed release AAB..."
"$UNITY" -batchmode -quit -nographics \
    -projectPath "$PROJECT_PATH" \
    -executeMethod EndlessRails.EditorTools.BuildScript.BuildAndroidAab \
    -logFile "$PROJECT_PATH/Builds/aab.log"

echo ""
echo "============================================================"
echo " Build complete!"
echo "   APK: $PROJECT_PATH/Builds/Android/EndlessRails.apk"
echo "   AAB: $PROJECT_PATH/Builds/Android/EndlessRails.aab"
echo "============================================================"
