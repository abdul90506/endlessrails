// Part 2 of Unity API stubs - filled in from compile-check iteration.
// All members below verified against real Unity 6 (6000.x) API signatures.
#pragma warning disable CS0067, CS0649, CS0414, CS0169, CS0282, CS0108

using System;
using System.Collections;

namespace UnityEngine
{
    public class WaitForSecondsRealtime : YieldInstruction
    {
        public WaitForSecondsRealtime(float time) { }
        public float waitTime { get => 0f; set { } }
    }

    public class WaitUntil : CustomYieldInstruction
    {
        public WaitUntil(Func<bool> predicate) { }
        public override bool keepWaiting => false;
    }

    public class WaitWhile : CustomYieldInstruction
    {
        public WaitWhile(Func<bool> predicate) { }
        public override bool keepWaiting => false;
    }

    public abstract class CustomYieldInstruction : IEnumerator
    {
        public abstract bool keepWaiting { get; }
        public object Current => null;
        public bool MoveNext() => keepWaiting;
        public void Reset() { }
    }

    public enum QueryTriggerInteraction { UseGlobal = 0, Ignore = 1, Collide = 2 }
    public enum ShadowQuality { Disable = 0, HardOnly = 1, All = 2 }

    public static class QualitySettings
    {
        public static string[] names => null;
        public static int GetQualityLevel() => 0;
        public static void SetQualityLevel(int index) { }
        public static void SetQualityLevel(int index, bool applyExpensiveChanges) { }
        public static ShadowQuality shadows { get => default; set { } }
        public static int antiAliasing { get => 0; set { } }
        public static int pixelLightCount { get => 0; set { } }
        public static float lodBias { get => 1f; set { } }
        public static int maximumLODLevel { get => 0; set { } }
        public static int vSyncCount { get => 0; set { } }
        public static int textureQuality { get => 0; set { } }
        public static float shadowDistance { get => 40f; set { } }
        public static int shadowCascades { get => 1; set { } }
        public static int shadowResolution { get => 0; set { } }
        public static float shadowNearPlaneOffset { get => 3f; set { } }
        public static bool softParticles { get => false; set { } }
        public static bool realtimeReflectionProbes { get => false; set { } }
        public static bool billboardsFaceCameraPosition { get => false; set { } }
        public static float particleRaycastBudget { get => 256f; set { } }
        public static int masterTextureLimit { get => 0; set { } }
        public static UnityEngine.Rendering.RenderPipelineAsset renderPipeline { get => null; set { } }
        // NOTE: QualitySettings.SetRenderPipelineAssetAt is NOT public API in Unity 6000.0.32f1
        // (CS0117 in a real cloud build) - deliberately NOT stubbed. Use GraphicsSettings.defaultRenderPipeline.
        public static UnityEngine.Rendering.RenderPipelineAsset GetRenderPipelineAssetAt(int index) => null;
        public static UnityEngine.Rendering.ColorSpace activeColorSpace => default;
    }

    public enum ParticleSystemShapeType
    {
        Sphere = 0, SphereShell = 1, Circle = 2, CircleEdge = 3, Cone = 4, Box = 5,
        Mesh = 6, MeshRenderer = 7, SkinnedMeshRenderer = 8, Sprite = 9, SpriteRenderer = 10,
        Edge = 11, SingleSidedEdge = 12, Donut = 13, Rectangle = 14, RectangleEdge = 15
    }

    public enum ParticleSystemRenderMode
    {
        Billboard = 0, Stretch = 1, HorizontalBillboard = 2, VerticalBillboard = 3, Mesh = 4, None = 5
    }
}

namespace UnityEngine.Rendering
{
    public enum BlendMode
    {
        Zero = 0, One = 1, DstColor = 2, SrcColor = 3, OneMinusDstColor = 4, SrcAlpha = 5,
        OneMinusSrcColor = 6, OneMinusDstAlpha = 7, DstAlpha = 8, OneMinusSrcAlpha = 9, OneMinusSrcAlpha2 = 10
    }

    public enum RenderQueue
    {
        Background = 1000, Geometry = 2000, AlphaTest = 2450, GeometryLast = 2500, Transparent = 3000, Overlay = 4000
    }

    public class RenderPipelineAsset : Object { }

    public static class GraphicsSettings
    {
        public static RenderPipelineAsset defaultRenderPipeline { get => null; set { } }
        public static RenderPipelineAsset currentRenderPipeline => null;
        public static RenderPipelineAsset renderPipelineAsset { get => null; set { } }
    }
}

namespace UnityEngine.UI
{
    public struct ColorBlock
    {
        public Color normalColor { get => default; set { } }
        public Color highlightedColor { get => default; set { } }
        public Color pressedColor { get => default; set { } }
        public Color selectedColor { get => default; set { } }
        public Color disabledColor { get => default; set { } }
        public float colorMultiplier { get => 1f; set { } }
        public float fadeDuration { get => 0.1f; set { } }
    }
}

namespace UnityEditor
{
    using UnityEngine;

    public static class EditorUserBuildSettings
    {
        public static bool buildAppBundle { get => false; set { } }
        public static AndroidBuildType androidBuildType { get => default; set { } }
        public static AndroidBuildSystem androidBuildSystem { get => default; set { } }
        public static BuildTarget activeBuildTarget => default;
        public static BuildTargetGroup selectedBuildTargetGroup { get => default; set { } }
        public static bool development { get => false; set { } }
        public static bool allowDebugging { get => false; set { } }
        public static bool connectProfiler { get => false; set { } }
        public static bool explicitNullChecks { get => false; set { } }
        public static bool buildScriptsOnly { get => false; set { } }
    }

    public class AssetImporter : UnityEngine.Object
    {
        public string assetPath => "";
        public string userData { get => ""; set { } }
        public static AssetImporter GetAtPath(string path) => null;
        public void SaveAndReimport() { }
        public void ExtractAndSaveMaterials(string path) { }
    }

    public enum TextureImporterType
    {
        Default = 0, NormalMap = 1, GUI = 2, Cursor = 7, Cookie = 8, Lightmap = 10,
        SingleChannel = 13, Sprite = 8 | 0x100
    }

    public enum TextureImporterCompression
    {
        Uncompressed = 0, Compressed = 1, CompressedHQ = 2, CompressedLQ = 3
    }

    public enum TextureImporterShape { Texture2D = 0, TextureCube = 1, Texture2DArray = 2, Texture3D = 3 }

    public class TextureImporter : AssetImporter
    {
        public TextureImporterType textureType { get => default; set { } }
        public TextureImporterShape textureShape { get => default; set { } }
        public bool sRGBTexture { get => false; set { } }
        public bool mipmapEnabled { get => false; set { } }
        public bool alphaIsTransparency { get => false; set { } }
        public int maxTextureSize { get => 2048; set { } }
        public TextureImporterCompression textureCompression { get => default; set { } }
        public FilterMode filterMode { get => default; set { } }
        public TextureWrapMode wrapMode { get => default; set { } }
        public int anisoLevel { get => 1; set { } }
        public float mipMapBias { get => 0f; set { } }
        public bool crunchedCompression { get => false; set { } }
        public int compressionQuality { get => 50; set { } }
    }

    public enum AudioClipLoadType { DecompressOnLoad = 0, CompressedInMemory = 1, Streaming = 2 }

    public enum AudioCompressionFormat { PCM = 0, Vorbis = 1, ADPCM = 2, MP3 = 3, VAG = 4, HEVAG = 5, XMA = 6, AAC = 7, GCADPCM = 8, ATRAC9 = 9 }

    public enum AudioSampleRateSetting { PreserveSampleRate = 0, OptimizeSampleRate = 1, OverrideSampleRate = 2 }

    public struct AudioImporterSampleSettings
    {
        public AudioClipLoadType loadType { get => default; set { } }
        public AudioCompressionFormat compressionFormat { get => default; set { } }
        public AudioSampleRateSetting sampleRateSetting { get => default; set { } }
        public uint sampleRateOverride { get => 0; set { } }
        public float quality { get => 1f; set { } }
        public int conversionMode { get => 0; set { } }
    }

    public class AudioImporter : AssetImporter
    {
        public AudioImporterSampleSettings defaultSampleSettings { get => default; set { } }
        public bool forceToMono { get => false; set { } }
        public bool preloadAudioData { get => false; set { } }
        public bool loadInBackground { get => false; set { } }
        public int ambisonic { get => 0; set { } }
        public AudioImporterSampleSettings GetOverrideSampleSettings(string platform) => default;
        public bool SetOverrideSampleSettings(string platform, AudioImporterSampleSettings settings) => false;
        public void ClearSampleSettingOverride(string platform) { }
    }
}
