// Minimal Unity API stubs for offline compile verification of the EndlessRails project.
// Mirrors real Unity 6 signatures for the API surface the project uses.
// NOT shipped with the game - only used by Tools compile check.
#pragma warning disable CS0067, CS0649, CS0414, CS0169, CS0282, CS8500, CS0108, CS0114

using System;
using System.Collections;
using System.Collections.Generic;

// ============================== UnityEngine ==============================
namespace UnityEngine
{
    public class Object
    {
        public string name { get; set; }
        public HideFlags hideFlags { get; set; }
        public static void Destroy(Object obj) { }
        public static void Destroy(Object obj, float t) { }
        public static void DestroyImmediate(Object obj) { }
        public static void DestroyImmediate(Object obj, bool allowDestroyingAssets) { }
        public static T Instantiate<T>(T original) where T : Object => original;
        public static T Instantiate<T>(T original, Transform parent) where T : Object => original;
        public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : Object => original;
        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object => original;
        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object => original;
        public static void DontDestroyOnLoad(Object target) { }
        public static T FindFirstObjectByType<T>() where T : Object => default;
        public static T FindAnyObjectByType<T>() where T : Object => default;
        public static T FindObjectOfType<T>() where T : Object => default;
        public static T[] FindObjectsOfType<T>() where T : Object => default;
        public static T[] FindObjectsByType<T>(FindObjectsSortMode mode) where T : Object => default;
        public static GameObject Find(string name) => null;
        public static GameObject[] FindGameObjectsWithTag(string tag) => null;
        public static GameObject FindWithTag(string tag) => null;
        public int GetInstanceID() => 0;
        public static implicit operator bool(Object exists) => exists != null;
        public static bool operator ==(Object x, Object y) => ReferenceEquals(x, y);
        public static bool operator !=(Object x, Object y) => !ReferenceEquals(x, y);
        public override bool Equals(object other) => ReferenceEquals(this, other);
        public override int GetHashCode() => base.GetHashCode();
    }

    public enum FindObjectsSortMode { None, InstanceID }
    public enum HideFlags { None, HideInHierarchy, HideInInspector, DontSaveInEditor, NotEditable, DontSaveInBuild, DontUnloadUnusedAsset, DontSave, HideAndDontSave }
    public enum PrimitiveType { Sphere, Capsule, Cylinder, Cube, Plane, Quad }
    public enum Space { World, Self }
    public enum RuntimeInitializeLoadType { AfterSceneLoad, BeforeSceneLoad, AfterAssembliesLoaded, BeforeSplashScreen, SubsystemRegistration }

    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
        public float this[int i] { get => 0; set { } }
        public float magnitude => 0f;
        public float sqrMagnitude => 0f;
        public Vector2 normalized => this;
        public void Set(float newX, float newY) { }
        public void Normalize() { }
        public static float Dot(Vector2 lhs, Vector2 rhs) => 0f;
        public static float Distance(Vector2 a, Vector2 b) => 0f;
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => a;
        public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t) => a;
        public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta) => current;
        public static Vector2 Scale(Vector2 a, Vector2 b) => a;
        public static Vector2 Min(Vector2 lhs, Vector2 rhs) => lhs;
        public static Vector2 Max(Vector2 lhs, Vector2 rhs) => lhs;
        public static Vector2 ClampMagnitude(Vector2 vector, float maxLength) => vector;
        public static Vector2 operator +(Vector2 a, Vector2 b) => a;
        public static Vector2 operator -(Vector2 a, Vector2 b) => a;
        public static Vector2 operator -(Vector2 a) => a;
        public static Vector2 operator *(Vector2 a, float d) => a;
        public static Vector2 operator *(float d, Vector2 a) => a;
        public static Vector2 operator /(Vector2 a, float d) => a;
        public static bool operator ==(Vector2 lhs, Vector2 rhs) => true;
        public static bool operator !=(Vector2 lhs, Vector2 rhs) => false;
        public static implicit operator Vector3(Vector2 v) => new Vector3(v.x, v.y, 0f);
        public static implicit operator Vector2(Vector3 v) => new Vector2(v.x, v.y);
        public override bool Equals(object other) => true;
        public override int GetHashCode() => 0;
        public static Vector2 zero => default;
        public static Vector2 one => new Vector2(1f, 1f);
        public static Vector2 up => new Vector2(0f, 1f);
        public static Vector2 down => new Vector2(0f, -1f);
        public static Vector2 left => new Vector2(-1f, 0f);
        public static Vector2 right => new Vector2(1f, 0f);
        public override string ToString() => "";
    }

    public struct Vector3
    {
        public float x, y, z;
        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        public Vector3(float x, float y) { this.x = x; this.y = y; this.z = 0f; }
        public float this[int i] { get => 0f; set { } }
        public float magnitude => 0f;
        public float sqrMagnitude => 0f;
        public Vector3 normalized => this;
        public void Set(float newX, float newY, float newZ) { }
        public void Normalize() { }
        public static float Dot(Vector3 lhs, Vector3 rhs) => 0f;
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs) => lhs;
        public static float Distance(Vector3 a, Vector3 b) => 0f;
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) => a;
        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t) => a;
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta) => current;
        public static Vector3 Scale(Vector3 a, Vector3 b) => a;
        public static Vector3 Min(Vector3 lhs, Vector3 rhs) => lhs;
        public static Vector3 Max(Vector3 lhs, Vector3 rhs) => lhs;
        public static Vector3 ClampMagnitude(Vector3 vector, float maxLength) => vector;
        public static Vector3 Normalize(Vector3 value) => value;
        public static float Angle(Vector3 from, Vector3 to) => 0f;
        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis) => 0f;
        public static Vector3 Project(Vector3 vector, Vector3 onNormal) => vector;
        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal) => vector;
        public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal) => inDirection;
        public static Vector3 Slerp(Vector3 a, Vector3 b, float t) => a;
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime) => current;
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed) => current;
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime) => current;
        public static Vector3 operator +(Vector3 a, Vector3 b) => a;
        public static Vector3 operator -(Vector3 a, Vector3 b) => a;
        public static Vector3 operator -(Vector3 a) => a;
        public static Vector3 operator *(Vector3 a, float d) => a;
        public static Vector3 operator *(float d, Vector3 a) => a;
        public static Vector3 operator /(Vector3 a, float d) => a;
        public static bool operator ==(Vector3 lhs, Vector3 rhs) => true;
        public static bool operator !=(Vector3 lhs, Vector3 rhs) => false;
        public override bool Equals(object other) => true;
        public override int GetHashCode() => 0;
        public static Vector3 zero => default;
        public static Vector3 one => new Vector3(1f, 1f, 1f);
        public static Vector3 up => new Vector3(0f, 1f, 0f);
        public static Vector3 down => new Vector3(0f, -1f, 0f);
        public static Vector3 left => new Vector3(-1f, 0f, 0f);
        public static Vector3 right => new Vector3(1f, 0f, 0f);
        public static Vector3 forward => new Vector3(0f, 0f, 1f);
        public static Vector3 back => new Vector3(0f, 0f, -1f);
        public override string ToString() => "";
    }

    public struct Vector4
    {
        public float x, y, z, w;
        public Vector4(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
        public static implicit operator Vector4(Vector3 v) => new Vector4(v.x, v.y, v.z, 0f);
        public static implicit operator Vector3(Vector4 v) => new Vector3(v.x, v.y, v.z);
        public static Vector4 operator *(Vector4 a, float d) => a;
        public static Vector4 operator +(Vector4 a, Vector4 b) => a;
    }

    public struct Quaternion
    {
        public float x, y, z, w;
        public Quaternion(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
        public static Quaternion identity => default;
        public Vector3 eulerAngles => default;
        public static Quaternion Euler(float x, float y, float z) => default;
        public static Quaternion Euler(Vector3 euler) => default;
        public static Quaternion AngleAxis(float angle, Vector3 axis) => default;
        public static Quaternion LookRotation(Vector3 forward) => default;
        public static Quaternion LookRotation(Vector3 forward, Vector3 upwards) => default;
        public static Quaternion Slerp(Quaternion a, Quaternion b, float t) => a;
        public static Quaternion Lerp(Quaternion a, Quaternion b, float t) => a;
        public static Quaternion Inverse(Quaternion rotation) => rotation;
        public static float Angle(Quaternion a, Quaternion b) => 0f;
        public static Quaternion operator *(Quaternion lhs, Quaternion rhs) => lhs;
        public static Vector3 operator *(Quaternion rotation, Vector3 point) => point;
        public void Set(float newX, float newY, float newZ, float newW) { }
        public void SetLookRotation(Vector3 view) { }
    }

    public struct Color
    {
        public float r, g, b, a;
        public Color(float r, float g, float b) { this.r = r; this.g = g; this.b = b; this.a = 1f; }
        public Color(float r, float g, float b, float a) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static Color Lerp(Color a, Color b, float t) => a;
        public static Color LerpUnclamped(Color a, Color b, float t) => a;
        public static Color operator +(Color a, Color b) => a;
        public static Color operator -(Color a, Color b) => a;
        public static Color operator *(Color a, float d) => a;
        public static Color operator *(float d, Color a) => a;
        public static Color operator *(Color a, Color b) => a;
        public static Color operator /(Color a, float d) => a;
        public static bool operator ==(Color lhs, Color rhs) => true;
        public static bool operator !=(Color lhs, Color rhs) => false;
        public override bool Equals(object other) => true;
        public override int GetHashCode() => 0;
        public static implicit operator Vector4(Color c) => new Vector4(c.r, c.g, c.b, c.a);
        public static implicit operator Color(Vector4 v) => new Color(v.x, v.y, v.z, v.w);
        public float this[int i] { get => 0f; set { } }
        public static Color red => new Color(1f, 0f, 0f);
        public static Color green => new Color(0f, 1f, 0f);
        public static Color blue => new Color(0f, 0f, 1f);
        public static Color white => new Color(1f, 1f, 1f);
        public static Color black => new Color(0f, 0f, 0f);
        public static Color yellow => new Color(1f, 0.92f, 0.016f);
        public static Color cyan => new Color(0f, 1f, 1f);
        public static Color magenta => new Color(1f, 0f, 1f);
        public static Color gray => new Color(0.5f, 0.5f, 0.5f);
        public static Color grey => new Color(0.5f, 0.5f, 0.5f);
        public static Color clear => new Color(0f, 0f, 0f, 0f);
        public override string ToString() => "";
    }

    public struct Color32
    {
        public byte r, g, b, a;
        public Color32(byte r, byte g, byte b, byte a) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static implicit operator Color32(Color c) => default;
        public static implicit operator Color(Color32 c) => default;
    }

    public struct Rect
    {
        public float x, y, width, height;
        public Rect(float x, float y, float width, float height) { this.x = x; this.y = y; this.width = width; this.height = height; }
        public float xMin { get => 0f; set { } }
        public float xMax { get => 0f; set { } }
        public float yMin { get => 0f; set { } }
        public float yMax { get => 0f; set { } }
        public Vector2 center { get => default; set { } }
        public Vector2 size { get => default; set { } }
        public Vector2 position { get => default; set { } }
        public bool Contains(Vector2 point) => false;
        public bool Contains(Vector3 point) => false;
    }

    public struct Bounds
    {
        public Vector3 center { get => default; set { } }
        public Vector3 size { get => default; set { } }
        public Vector3 extents { get => default; set { } }
        public Vector3 min { get => default; set { } }
        public Vector3 max { get => default; set { } }
        public Bounds(Vector3 center, Vector3 size) { }
        public void Encapsulate(Bounds bounds) { }
        public void Encapsulate(Vector3 point) { }
        public bool Intersects(Bounds bounds) => false;
        public bool Contains(Vector3 point) => false;
        public float SqrDistance(Vector3 point) => 0f;
    }

    public struct Matrix4x4
    {
        public static Matrix4x4 identity => default;
        public static Matrix4x4 TRS(Vector3 pos, Quaternion q, Vector3 s) => default;
        public Vector3 MultiplyPoint(Vector3 point) => point;
        public Vector3 MultiplyPoint3x4(Vector3 point) => point;
        public Vector3 MultiplyVector(Vector3 vector) => vector;
    }

    public static class Mathf
    {
        public const float PI = 3.14159265f;
        public const float Infinity = float.PositiveInfinity;
        public const float NegativeInfinity = float.NegativeInfinity;
        public const float Deg2Rad = 0.0174532924f;
        public const float Rad2Deg = 57.29578f;
        public const float Epsilon = 1.401298E-45f;
        public static float Abs(float f) => 0f;
        public static int Abs(int value) => 0;
        public static float Min(float a, float b) => 0f;
        public static int Min(int a, int b) => 0;
        public static float Min(params float[] values) => 0f;
        public static int Min(params int[] values) => 0;
        public static float Max(float a, float b) => 0f;
        public static int Max(int a, int b) => 0;
        public static float Max(params float[] values) => 0f;
        public static int Max(params int[] values) => 0;
        public static float Pow(float f, float p) => 0f;
        public static float Exp(float power) => 0f;
        public static float Log(float f) => 0f;
        public static float Log(float f, float p) => 0f;
        public static float Log10(float f) => 0f;
        public static float Sqrt(float f) => 0f;
        public static float Sin(float f) => 0f;
        public static float Cos(float f) => 0f;
        public static float Tan(float f) => 0f;
        public static float Asin(float f) => 0f;
        public static float Acos(float f) => 0f;
        public static float Atan(float f) => 0f;
        public static float Atan2(float y, float x) => 0f;
        public static float Clamp(float value, float min, float max) => 0f;
        public static int Clamp(int value, int min, int max) => 0;
        public static float Clamp01(float value) => 0f;
        public static float Lerp(float a, float b, float t) => 0f;
        public static float LerpUnclamped(float a, float b, float t) => 0f;
        public static float LerpAngle(float a, float b, float t) => 0f;
        public static float InverseLerp(float a, float b, float value) => 0f;
        public static float MoveTowards(float current, float target, float maxDelta) => 0f;
        public static float MoveTowardsAngle(float current, float target, float maxDelta) => 0f;
        public static float SmoothStep(float from, float to, float t) => 0f;
        public static float Gamma(float value, float absmax, float gamma) => 0f;
        public static bool Approximately(float a, float b) => false;
        public static float Repeat(float t, float length) => 0f;
        public static float PingPong(float t, float length) => 0f;
        public static float Floor(float f) => 0f;
        public static int FloorToInt(float f) => 0;
        public static float Ceil(float f) => 0f;
        public static int CeilToInt(float f) => 0;
        public static float Round(float f) => 0f;
        public static int RoundToInt(float f) => 0;
        public static float Sign(float f) => 0f;
        public static float PerlinNoise(float x, float y) => 0f;
        public static float DeltaAngle(float current, float target) => 0f;
        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime) => 0f;
        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime) => 0f;
    }

    public static class Random
    {
        public static float value => 0f;
        public static float Range(float min, float max) => 0f;
        public static int Range(int min, int max) => 0;
        public static Vector3 insideUnitSphere => default;
        public static Vector3 onUnitSphere => default;
        public static Vector2 insideUnitCircle => default;
        public static Quaternion rotation => default;
        public static void InitState(int seed) { }
    }

    public static class Time
    {
        public static float time => 0f;
        public static float unscaledTime => 0f;
        public static float timeSinceLevelLoad => 0f;
        public static float deltaTime => 0f;
        public static float fixedDeltaTime { get => 0f; set { } }
        public static float fixedTime => 0f;
        public static float unscaledDeltaTime => 0f;
        public static float smoothDeltaTime => 0f;
        public static float timeScale { get => 1f; set { } }
        public static float realtimeSinceStartup => 0f;
        public static double realtimeSinceStartupAsDouble => 0d;
        public static int frameCount => 0;
        public static int capturedFrameCount => 0;
        public static float maximumDeltaTime => 0f;
    }

    public enum KeyCode
    {
        None, Space, Return, Escape, Backspace, Tab, Delete, UpArrow, DownArrow, LeftArrow, RightArrow,
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        Alpha0, Alpha1, Alpha2, Alpha3, Alpha4, Alpha5, Alpha6, Alpha7, Alpha8, Alpha9,
        LeftShift, RightShift, LeftControl, RightControl, LeftAlt, RightAlt, Mouse0, Mouse1, Mouse2
    }

    public enum TouchPhase { Began, Moved, Stationary, Ended, Canceled }

    public struct Touch
    {
        public int fingerId => 0;
        public Vector2 position => default;
        public Vector2 rawPosition => default;
        public Vector2 deltaPosition => default;
        public float deltaTime => 0f;
        public int tapCount => 0;
        public TouchPhase phase => default;
    }

    public static class Input
    {
        public static Vector3 mousePosition => default;
        public static int touchCount => 0;
        public static bool multiTouchEnabled { get => false; set { } }
        public static bool simulateMouseWithTouches { get => false; set { } }
        public static Touch GetTouch(int index) => default;
        public static bool GetKey(KeyCode key) => false;
        public static bool GetKeyDown(KeyCode key) => false;
        public static bool GetKeyUp(KeyCode key) => false;
        public static bool GetKey(string name) => false;
        public static bool GetKeyDown(string name) => false;
        public static bool GetMouseButton(int button) => false;
        public static bool GetMouseButtonDown(int button) => false;
        public static bool GetMouseButtonUp(int button) => false;
        public static float GetAxis(string axisName) => 0f;
        public static float GetAxisRaw(string axisName) => 0f;
        public static bool GetButton(string buttonName) => false;
        public static bool GetButtonDown(string buttonName) => false;
        public static bool GetButtonUp(string buttonName) => false;
        public static bool anyKey => false;
        public static bool anyKeyDown => false;
    }

    public static class Screen
    {
        public static int width => 0;
        public static int height => 0;
        public static float dpi => 0f;
        public static bool fullScreen { get => false; set { } }
        public static Rect safeArea => default;
        public static ScreenOrientation orientation { get => default; set { } }
        public static int sleepTimeout { get => 0; set { } }
    }

    public enum ScreenOrientation { Portrait, PortraitUpsideDown, LandscapeLeft, LandscapeRight, AutoRotation }

    public static class Application
    {
        public delegate void QuitCallback();
        public delegate void LowMemoryCallback();
        public static string persistentDataPath => "";
        public static string dataPath => "";
        public static string streamingAssetsPath => "";
        public static string productName => "";
        public static string companyName => "";
        public static string version => "";
        public static string unityVersion => "";
        public static bool isEditor => false;
        public static bool isBatchMode => false;
        public static bool isMobilePlatform => false;
        public static bool isPlaying => false;
        public static RuntimePlatform platform => default;
        public static int targetFrameRate { get => 0; set { } }
        public static bool runInBackground { get => false; set { } }
        public static event QuitCallback quitting;
        public static event LowMemoryCallback lowMemory;
        public static void Quit() { }
        public static void Quit(int exitCode) { }
    }

    public enum RuntimePlatform { OSXEditor, OSXPlayer, WindowsPlayer, WindowsEditor, IPhonePlayer, Android, LinuxPlayer, LinuxEditor, WebGLPlayer, WSAPlayerX86, WSAPlayerX64, WSAPlayerARM, PS4, XboxOne, tvOS, Switch, Lumin }

    public static class Debug
    {
        public static void Log(object message) { }
        public static void Log(object message, Object context) { }
        public static void LogWarning(object message) { }
        public static void LogWarning(object message, Object context) { }
        public static void LogError(object message) { }
        public static void LogError(object message, Object context) { }
        public static void LogException(Exception exception) { }
        public static void LogException(Exception exception, Object context) { }
        public static void LogFormat(string format, params object[] args) { }
        public static void LogWarningFormat(string format, params object[] args) { }
        public static void LogErrorFormat(string format, params object[] args) { }
        public static void Assert(bool condition) { }
        public static void Assert(bool condition, string message) { }
        public static void DrawLine(Vector3 start, Vector3 end) { }
        public static void DrawLine(Vector3 start, Vector3 end, Color color) { }
    }

    public static class JsonUtility
    {
        public static string ToJson(object obj) => "";
        public static string ToJson(object obj, bool prettyPrint) => "";
        public static T FromJson<T>(string json) => default;
        public static object FromJson(string json, Type type) => null;
        public static void FromJsonOverwrite(string json, object objectToOverwrite) { }
    }

    public class Component : Object
    {
        public GameObject gameObject => null;
        public Transform transform => null;
        public string tag { get => ""; set { } }
        public T GetComponent<T>() => default;
        public Component GetComponent(Type type) => null;
        public Component GetComponent(string type) => null;
        public T GetComponentInParent<T>() => default;
        public T GetComponentInParent<T>(bool includeInactive) => default;
        public T GetComponentInChildren<T>() => default;
        public T GetComponentInChildren<T>(bool includeInactive) => default;
        public T[] GetComponents<T>() => default;
        public T[] GetComponentsInChildren<T>() => default;
        public T[] GetComponentsInChildren<T>(bool includeInactive) => default;
        public T[] GetComponentsInParent<T>() => default;
        public T[] GetComponentsInParent<T>(bool includeInactive) => default;
        public bool TryGetComponent<T>(out T component) { component = default; return false; }
        public bool CompareTag(string tag) => false;
        public void SendMessage(string methodName) { }
        public void SendMessage(string methodName, object value) { }
    }

    public class Behaviour : Component
    {
        public bool enabled { get => false; set { } }
        public bool isActiveAndEnabled => false;
    }

    public class Coroutine : YieldInstruction { }
    public class YieldInstruction { }
    public class WaitForSeconds : YieldInstruction { public WaitForSeconds(float seconds) { } }
    public class WaitForEndOfFrame : YieldInstruction { }
    public class WaitForFixedUpdate : YieldInstruction { }
    public class AsyncOperation : YieldInstruction
    {
        public bool isDone => false;
        public float progress => 0f;
        public bool allowSceneActivation { get => false; set { } }
    }

    public class MonoBehaviour : Behaviour
    {
        public Coroutine StartCoroutine(IEnumerator routine) => null;
        public Coroutine StartCoroutine(string methodName) => null;
        public void StopCoroutine(IEnumerator routine) { }
        public void StopCoroutine(Coroutine routine) { }
        public void StopCoroutine(string methodName) { }
        public void StopAllCoroutines() { }
        public void Invoke(string methodName, float time) { }
        public void InvokeRepeating(string methodName, float time, float repeatRate) { }
        public void CancelInvoke() { }
        public void CancelInvoke(string methodName) { }
        public bool IsInvoking() => false;
        public bool useGUILayout { get => false; set { } }
    }

    public class GameObject : Object
    {
        public GameObject() { }
        public GameObject(string name) { }
        public GameObject(string name, params Type[] components) { }
        public Transform transform => null;
        public int layer { get => 0; set { } }
        public string tag { get => ""; set { } }
        public bool activeSelf => false;
        public bool activeInHierarchy => false;
        public bool isStatic { get => false; set { } }
        public void SetActive(bool value) { }
        public T GetComponent<T>() => default;
        public Component GetComponent(Type type) => null;
        public Component GetComponent(string type) => null;
        public T GetComponentInParent<T>() => default;
        public T GetComponentInChildren<T>() => default;
        public T[] GetComponents<T>() => default;
        public T[] GetComponentsInChildren<T>() => default;
        public T[] GetComponentsInChildren<T>(bool includeInactive) => default;
        public bool TryGetComponent<T>(out T component) { component = default; return false; }
        public T AddComponent<T>() where T : Component => default;
        public Component AddComponent(Type componentType) => null;
        public bool CompareTag(string tag) => false;
        public static GameObject CreatePrimitive(PrimitiveType type) => null;
        public static GameObject Find(string name) => null;
        public void SendMessage(string methodName) { }
    }

    public class Transform : Component, IEnumerable
    {
        public Vector3 position { get => default; set { } }
        public Vector3 localPosition { get => default; set { } }
        public Quaternion rotation { get => default; set { } }
        public Quaternion localRotation { get => default; set { } }
        public Vector3 localScale { get => default; set { } }
        public Vector3 lossyScale => default;
        public Vector3 eulerAngles { get => default; set { } }
        public Vector3 localEulerAngles { get => default; set { } }
        public Vector3 forward { get => default; set { } }
        public Vector3 up { get => default; set { } }
        public Vector3 right { get => default; set { } }
        public Transform parent { get => null; set { } }
        public Transform root => null;
        public int childCount => 0;
        public void SetParent(Transform p) { }
        public void SetParent(Transform parent, bool worldPositionStays) { }
        public void SetParent(GameObject parent, bool worldPositionStays) { }
        public Transform Find(string name) => null;
        public Transform GetChild(int index) => null;
        public int GetSiblingIndex() => 0;
        public void SetSiblingIndex(int index) { }
        public void SetAsFirstSibling() { }
        public void SetAsLastSibling() { }
        public bool IsChildOf(Transform parent) => false;
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation) { }
        public void Translate(Vector3 translation) { }
        public void Translate(Vector3 translation, Space relativeTo) { }
        public void Translate(float x, float y, float z) { }
        public void Rotate(Vector3 eulers) { }
        public void Rotate(Vector3 eulers, Space relativeTo) { }
        public void Rotate(Vector3 axis, float angle) { }
        public void Rotate(Vector3 axis, float angle, Space relativeTo) { }
        public void RotateAround(Vector3 point, Vector3 axis, float angle) { }
        public void LookAt(Transform target) { }
        public void LookAt(Vector3 worldPosition) { }
        public Vector3 TransformPoint(Vector3 position) => default;
        public Vector3 InverseTransformPoint(Vector3 position) => default;
        public Vector3 TransformDirection(Vector3 direction) => default;
        public Vector3 InverseTransformDirection(Vector3 direction) => default;
        public Vector3 TransformVector(Vector3 vector) => default;
        public Vector3 InverseTransformVector(Vector3 vector) => default;
        public IEnumerator GetEnumerator() => null;
    }

    public class RectTransform : Transform
    {
        public Vector2 anchorMin { get => default; set { } }
        public Vector2 anchorMax { get => default; set { } }
        public Vector2 anchoredPosition { get => default; set { } }
        public Vector2 anchoredPosition3D { get => default; set { } }
        public Vector2 sizeDelta { get => default; set { } }
        public Vector2 pivot { get => default; set { } }
        public Vector2 offsetMin { get => default; set { } }
        public Vector2 offsetMax { get => default; set { } }
        public Rect rect => default;
        public void SetSizeWithCurrentAnchors(Axis axis, float size) { }
        public void SetInsetAndSizeFromParentEdge(Edge edge, float inset, float size) { }
        public void GetLocalCorners(Vector3[] fourCornersArray) { }
        public void GetWorldCorners(Vector3[] fourCornersArray) { }
        public void ForceUpdateRectTransforms() { }
    }

    public enum Axis { Horizontal, Vertical }
    public enum Edge { Left, Right, Top, Bottom }

    public class CanvasRenderer : Component
    {
        public float GetAlpha() => 0f;
        public void SetAlpha(float alpha) { }
    }

    public class Canvas : Behaviour
    {
        public RenderMode renderMode { get => default; set { } }
        public Camera worldCamera { get => null; set { } }
        public float planeDistance { get => 0f; set { } }
        public int sortingOrder { get => 0; set { } }
        public int sortingLayerID { get => 0; set { } }
        public string sortingLayerName { get => ""; set { } }
        public bool overrideSorting { get => false; set { } }
        public bool pixelPerfect { get => false; set { } }
        public float scaleFactor { get => 1f; set { } }
        public static void ForceUpdateCanvases() { }
    }

    public enum RenderMode { ScreenSpaceOverlay, ScreenSpaceCamera, WorldSpace }

    public class CanvasGroup : Component
    {
        public float alpha { get => 1f; set { } }
        public bool interactable { get => false; set { } }
        public bool blocksRaycasts { get => false; set { } }
        public bool ignoreParentGroups { get => false; set { } }
    }

    public class Sprite : Object
    {
        public Rect rect => default;
        public float pixelsPerUnit => 0f;
        public Texture2D texture => null;
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot) => null;
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit) => null;
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude) => null;
    }

    public class Texture : Object
    {
        public int width { get => 0; set { } }
        public int height { get => 0; set { } }
        public FilterMode filterMode { get => default; set { } }
        public TextureWrapMode wrapMode { get => default; set { } }
        public int anisoLevel { get => 0; set { } }
    }

    public enum FilterMode { Point, Bilinear, Trilinear }
    public enum TextureWrapMode { Repeat, Clamp, Mirror, MirrorOnce }

    public class Texture2D : Texture
    {
        public Texture2D(int width, int height) { }
        public Texture2D(int width, int height, TextureFormat textureFormat, bool mipChain) { }
        public TextureFormat format => default;
        public void SetPixel(int x, int y, Color color) { }
        public Color GetPixel(int x, int y) => default;
        public void SetPixels(Color[] colors) { }
        public Color[] GetPixels() => null;
        public void Apply() { }
        public void Apply(bool updateMipmaps) { }
        public byte[] EncodeToPNG() => null;
        public bool LoadImage(byte[] data) => false;
        public static Texture2D whiteTexture => null;
        public static Texture2D blackTexture => null;
    }

    public enum TextureFormat { Alpha8, ARGB4444, RGB24, RGBA32, ARGB32, RGB565, R16, DXT1, DXT5, RGBA4444, BGRA32, RHalf, RGHalf, RGBAHalf, RFloat, RGFloat, RGBAFloat, RGB9E5, BC4, BC5, BC6H, BC7, DXT1Crunched, DXT5Crunched, PVRTC_RGB2, PVRTC_RGBA2, PVRTC_RGB4, PVRTC_RGBA4, ETC_RGB4, EAC_R, EAC_R_SIGNED, EAC_RG, EAC_RG_SIGNED, ETC2_RGB, ETC2_RGBA1, ETC2_RGBA8, ASTC_4x4, ASTC_5x5, ASTC_6x6, ASTC_8x8, ASTC_10x10, ASTC_12x12, ETC_RGB4Crunched, ETC2_RGBA8Crunched }

    public class RenderTexture : Texture
    {
        public RenderTexture(int width, int height, int depth) { }
        public static RenderTexture active { get => null; set { } }
        public bool Create() => false;
        public void Release() { }
    }

    public class Shader : Object
    {
        public static Shader Find(string name) => null;
        public int propertyToID(string name) => 0;
    }

    public class Material : Object
    {
        public Material(Shader shader) { }
        public Material(Material source) { }
        public Color color { get => default; set { } }
        public Texture mainTexture { get => default; set { } }
        public Shader shader { get => default; set { } }
        public bool enableInstancing { get => false; set { } }
        public int renderQueue { get => 0; set { } }
        public void SetFloat(string name, float value) { }
        public void SetInt(string name, int value) { }
        public void SetColor(string name, Color value) { }
        public void SetVector(string name, Vector4 value) { }
        public void SetTexture(string name, Texture value) { }
        public float GetFloat(string name) => 0f;
        public Color GetColor(string name) => default;
        public bool HasProperty(string name) => false;
        public void EnableKeyword(string keyword) { }
        public void DisableKeyword(string keyword) { }
        public void SetOverrideTag(string tag, string val) { }
    }

    public class Mesh : Object
    {
        public Vector3[] vertices { get => null; set { } }
        public Vector3[] normals { get => null; set { } }
        public Vector2[] uv { get => null; set { } }
        public Color[] colors { get => null; set { } }
        public int[] triangles { get => null; set { } }
        public Bounds bounds => default;
        public void RecalculateNormals() { }
        public void RecalculateBounds() { }
        public void Clear() { }
    }

    public class MeshFilter : Component
    {
        public Mesh mesh { get => null; set { } }
        public Mesh sharedMesh { get => null; set { } }
    }

    public class Renderer : Component
    {
        public Material material { get => null; set { } }
        public Material sharedMaterial { get => null; set { } }
        public Material[] materials { get => null; set { } }
        public Material[] sharedMaterials { get => null; set { } }
        public bool enabled { get => false; set { } }
        public Bounds bounds => default;
        public int sortingOrder { get => 0; set { } }
        public int sortingLayerID { get => 0; set { } }
        public string sortingLayerName { get => ""; set { } }
        public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode { get => default; set { } }
        public bool receiveShadows { get => false; set { } }
    }

    public class MeshRenderer : Renderer { }
    public class SkinnedMeshRenderer : Renderer { public Mesh sharedMesh { get => null; set { } } }
    public class SpriteRenderer : Renderer { public Sprite sprite { get => null; set { } } }
    public class LineRenderer : Renderer { }

    public class ShaderVariantCollection : Object { }

    public class Camera : Behaviour
    {
        public static Camera main => null;
        public float fieldOfView { get => 0f; set { } }
        public float nearClipPlane { get => 0f; set { } }
        public float farClipPlane { get => 0f; set { } }
        public float orthographicSize { get => 0f; set { } }
        public bool orthographic { get => false; set { } }
        public Color backgroundColor { get => default; set { } }
        public bool allowHDR { get => false; set { } }
        public bool allowMSAA { get => false; set { } }
        public bool clearFlags { get => false; set { } }
        public DepthTextureMode depthTextureMode { get => default; set { } }
        public Ray ScreenPointToRay(Vector3 pos) => default;
        public Ray ViewportPointToRay(Vector3 pos) => default;
        public Vector3 WorldToScreenPoint(Vector3 position) => default;
        public Vector3 WorldToViewportPoint(Vector3 position) => default;
        public Vector3 ScreenToWorldPoint(Vector3 position) => default;
        public Vector3 ViewportToWorldPoint(Vector3 position) => default;
    }

    [Flags]
    public enum DepthTextureMode { None = 0, Depth = 1, DepthNormals = 2, MotionVectors = 4 }

    public struct Ray
    {
        public Ray(Vector3 origin, Vector3 direction) { }
        public Vector3 origin { get => default; set { } }
        public Vector3 direction { get => default; set { } }
        public Vector3 GetPoint(float distance) => default;
    }

    public struct RaycastHit
    {
        public Vector3 point => default;
        public Vector3 normal => default;
        public Vector3 barycentricCoordinate => default;
        public float distance => 0f;
        public Collider collider => null;
        public Transform transform => null;
        public Rigidbody rigidbody => null;
    }

    public static class Physics
    {
        public const int DefaultRaycastLayers = -5;
        public const int AllLayers = -1;
        public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance) { hitInfo = default; return false; }
        public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask) { hitInfo = default; return false; }
        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance) { hitInfo = default; return false; }
        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask) { hitInfo = default; return false; }
        public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance) => false;
        public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo) { hitInfo = default; return false; }
        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance) { hitInfo = default; return false; }
        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance) { hitInfo = default; return false; }
        public static Collider[] OverlapSphere(Vector3 position, float radius) => null;
        public static Collider[] OverlapSphere(Vector3 position, float radius, int layerMask) => null;
        public static Collider[] OverlapSphere(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction) => null;
        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction) { hitInfo = default; return false; }
        public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction) { hitInfo = default; return false; }
        public static bool CheckSphere(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction) => false;
        public static void IgnoreCollision(Collider collider1, Collider collider2) { }
        public static void IgnoreCollision(Collider collider1, Collider collider2, bool ignore) { }
        public static void IgnoreLayerCollision(int layer1, int layer2, bool ignore) { }
        public static bool GetIgnoreLayerCollision(int layer1, int layer2) => false;
    }

    public struct LayerMask
    {
        public int value { get => 0; set { } }
        public static int NameToLayer(string layerName) => 0;
        public static string LayerToName(int layer) => "";
        public static int GetMask(params string[] layerNames) => 0;
        public static implicit operator int(LayerMask mask) => 0;
        public static implicit operator LayerMask(int intVal) => default;
    }

    public class Collider : Component
    {
        public bool isTrigger { get => false; set { } }
        public bool enabled { get => false; set { } }
        public Bounds bounds => default;
        public Rigidbody attachedRigidbody => null;
        public PhysicMaterial material { get => null; set { } }
        public Vector3 ClosestPoint(Vector3 position) => default;
        public void ClosestPointOnBounds(Vector3 position) { }
    }

    public class BoxCollider : Collider
    {
        public Vector3 center { get => default; set { } }
        public Vector3 size { get => default; set { } }
    }

    public class SphereCollider : Collider
    {
        public Vector3 center { get => default; set { } }
        public float radius { get => 0f; set { } }
    }

    public class CapsuleCollider : Collider
    {
        public Vector3 center { get => default; set { } }
        public float radius { get => 0f; set { } }
        public float height { get => 0f; set { } }
        public int direction { get => 0; set { } }
    }

    public class MeshCollider : Collider
    {
        public Mesh sharedMesh { get => null; set { } }
    }

    public class PhysicMaterial : Object { }

    public class Collision
    {
        public Collider collider => null;
        public Rigidbody rigidbody => null;
        public Transform transform => null;
        public int contactCount => 0;
        public ContactPoint[] contacts => null;
        public Vector3 relativeVelocity => default;
    }

    public struct ContactPoint
    {
        public Vector3 point => default;
        public Vector3 normal => default;
        public Collider otherCollider => null;
    }

    public enum ForceMode { Force, Acceleration, Impulse, VelocityChange }
    public enum RigidbodyConstraints { None, FreezePositionX, FreezePositionY, FreezePositionZ, FreezeRotationX, FreezeRotationY, FreezeRotationZ, FreezePosition, FreezeRotation, FreezeAll }
    public enum CollisionDetectionMode { Discrete, Continuous, ContinuousDynamic, ContinuousSpeculative }

    public class Rigidbody : Component
    {
        public Vector3 velocity { get => default; set { } }
        public Vector3 angularVelocity { get => default; set { } }
        public float mass { get => 1f; set { } }
        public float drag { get => 0f; set { } }
        public float angularDrag { get => 0.05f; set { } }
        public bool useGravity { get => false; set { } }
        public bool isKinematic { get => false; set { } }
        public RigidbodyConstraints constraints { get => default; set { } }
        public CollisionDetectionMode collisionDetectionMode { get => default; set { } }
        public bool detectCollisions { get => false; set { } }
        public bool freezeRotation { get => false; set { } }
        public Vector3 position { get => default; set { } }
        public Quaternion rotation { get => default; set { } }
        public RigidbodyInterpolation interpolation { get => default; set { } }
        public void AddForce(Vector3 force) { }
        public void AddForce(Vector3 force, ForceMode mode) { }
        public void AddForce(float x, float y, float z) { }
        public void AddForce(float x, float y, float z, ForceMode mode) { }
        public void AddRelativeForce(Vector3 force) { }
        public void AddRelativeForce(Vector3 force, ForceMode mode) { }
        public void AddTorque(Vector3 torque) { }
        public void AddTorque(Vector3 torque, ForceMode mode) { }
        public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius) { }
        public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier) { }
        public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode mode) { }
        public void MovePosition(Vector3 position) { }
        public void MoveRotation(Quaternion newRotation) { }
        public Vector3 GetPointVelocity(Vector3 worldPoint) => default;
        public void Sleep() { }
        public void WakeUp() { }
    }

    public enum RigidbodyInterpolation { None, Interpolate, Extrapolate }

    public class CharacterController : Collider
    {
        public Vector3 velocity => default;
        public bool isGrounded => false;
        public CollisionFlags Move(Vector3 motion) => default;
        public bool SimpleMove(Vector3 speed) => false;
        public float radius { get => 0f; set { } }
        public float height { get => 0f; set { } }
        public Vector3 center { get => default; set { } }
        public float slopeLimit { get => 0f; set { } }
        public float stepOffset { get => 0f; set { } }
        public bool detectCollisions { get => false; set { } }
        public bool enableOverlapRecovery { get => false; set { } }
    }

    [Flags]
    public enum CollisionFlags { None = 0, Sides = 1, Above = 2, Below = 4 }

    public class Light : Behaviour
    {
        public LightType type { get => default; set { } }
        public Color color { get => default; set { } }
        public float intensity { get => 1f; set { } }
        public float range { get => 10f; set { } }
        public float spotAngle { get => 30f; set { } }
        public LightShadows shadows { get => default; set { } }
        public float shadowStrength { get => 1f; set { } }
        public float shadowBias { get => 0f; set { } }
        public int shadowResolution { get => 0; set { } }
        public float shadowNearPlane { get => 0f; set { } }
        public int cullingMask { get => 0; set { } }
    }

    public enum LightType { Spot, Directional, Point, Area, Rectangle, Disc, Pyramid }
    public enum LightShadows { None, Hard, Soft }

    public class Skybox : Behaviour
    {
        public Material material { get => null; set { } }
    }

    public static class RenderSettings
    {
        public static bool fog { get => false; set { } }
        public static Color fogColor { get => default; set { } }
        public static FogMode fogMode { get => default; set { } }
        public static float fogDensity { get => 0f; set { } }
        public static float fogStartDistance { get => 0f; set { } }
        public static float fogEndDistance { get => 0f; set { } }
        public static Color ambientLight { get => default; set { } }
        public static Color ambientSkyColor { get => default; set { } }
        public static Color ambientEquatorColor { get => default; set { } }
        public static Color ambientGroundColor { get => default; set { } }
        public static UnityEngine.Rendering.AmbientMode ambientMode { get => default; set { } }
        public static float ambientIntensity { get => 1f; set { } }
        public static Material skybox { get => null; set { } }
        public static float reflectionIntensity { get => 1f; set { } }
    }

    public enum FogMode { Linear = 1, Exponential = 2, ExponentialSquared = 3 }

    public class ParticleSystem : Component
    {
        public MainModule main => default;
        public EmissionModule emission => default;
        public ShapeModule shape => default;
        public ColorOverLifetimeModule colorOverLifetime => default;
        public bool isPlaying => false;
        public bool isEmitting => false;
        public void Play() { }
        public void Play(bool withChildren) { }
        public void Stop() { }
        public void Stop(bool withChildren) { }
        public void Stop(bool withChildren, ParticleSystemStopBehavior stopBehavior) { }
        public void Clear() { }
        public void Emit(int count) { }
        public void Simulate(float t) { }

        public struct MainModule
        {
            public float duration { get => 0f; set { } }
            public bool loop { get => false; set { } }
            public bool playOnAwake { get => false; set { } }
            public float startSpeed { get => 0f; set { } }
            public float startSize { get => 0f; set { } }
            public float startLifetime { get => 0f; set { } }
            public MinMaxGradient startColor { get => default; set { } }
            public ParticleSystemSimulationSpace simulationSpace { get => default; set { } }
            public int maxParticles { get => 0; set { } }
            public float gravityModifier { get => 0f; set { } }
        }

        public struct EmissionModule
        {
            public bool enabled { get => false; set { } }
            public MinMaxCurve rateOverTime { get => default; set { } }
            public MinMaxCurve rateOverDistance { get => default; set { } }
        }

        public struct ShapeModule
        {
            public bool enabled { get => false; set { } }
            public ParticleSystemShapeType shapeType { get => default; set { } }
            public float radius { get => 0f; set { } }
            public Vector3 scale { get => default; set { } }
            public Vector3 position { get => default; set { } }
        }

        public struct ColorOverLifetimeModule
        {
            public bool enabled { get => false; set { } }
            public MinMaxGradient color { get => default; set { } }
        }

        public struct MinMaxGradient
        {
            public MinMaxGradient(Color min, Color max) { }
            public Color colorMax => default;
            public Color colorMin => default;
        }

        public struct MinMaxCurve
        {
            public MinMaxCurve(float constant) { }
            public MinMaxCurve(float multiplier, AnimationCurve curve) { }
            public float constantMax => 0f;
            public float constantMin => 0f;
            public static implicit operator MinMaxCurve(float constant) => default;
        }
    }

    public enum ParticleSystemStopBehavior { StopEmittingAndClear, StopEmitting }
    public enum ParticleSystemSimulationSpace { Local, World, Custom }

    public class ParticleSystemRenderer : Renderer
    {
        public ParticleSystemRenderMode renderMode { get => default; set { } }
        public ParticleSystemRenderSpace renderSpace { get => default; set { } }
        public float lengthScale { get => 0f; set { } }
        public float velocityScale { get => 0f; set { } }
    }

    public enum ParticleSystemRenderSpace { View, World, Local, Facing, Velocity }

    public class TrailRenderer : Renderer { }

    public class AnimationCurve
    {
        public AnimationCurve(params Keyframe[] keys) { }
        public Keyframe[] keys { get => null; set { } }
        public float Evaluate(float time) => 0f;
        public void AddKey(float time, float value) { }
        public void AddKey(Keyframe key) { }
        public int length => 0;
        public static AnimationCurve EaseInOut(float timeStart, float valueStart, float timeEnd, float valueEnd) => null;
        public static AnimationCurve Linear(float timeStart, float valueStart, float timeEnd, float valueEnd) => null;
    }

    public struct Keyframe
    {
        public Keyframe(float time, float value) { }
        public Keyframe(float time, float value, float inTangent, float outTangent) { }
        public float time { get => 0f; set { } }
        public float value { get => 0f; set { } }
        public float inTangent { get => 0f; set { } }
        public float outTangent { get => 0f; set { } }
    }

    public enum AudioRolloffMode { Logarithmic, Linear, Custom }
    public enum AudioVelocityUpdateMode { Auto, Fixed, Dynamic }

    public class AudioClip : Object
    {
        public float length => 0f;
        public int samples => 0;
        public int channels => 0;
        public int frequency => 0;
        public bool loadInBackground => false;
        public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream) => null;
        public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream, PCMReaderCallback pcmreadercallback) => null;
        public bool SetData(float[] data, int offsetSamples) => false;
        public bool GetData(float[] data, int offsetSamples) => false;
        public delegate void PCMReaderCallback(float[] data);
    }

    public class AudioListener : Behaviour
    {
        public static float volume { get => 0f; set { } }
        public static bool pause { get => false; set { } }
    }

    public class AudioSource : Behaviour
    {
        public AudioClip clip { get => null; set { } }
        public bool playOnAwake { get => false; set { } }
        public bool loop { get => false; set { } }
        public bool mute { get => false; set { } }
        public float volume { get => 1f; set { } }
        public float pitch { get => 1f; set { } }
        public float time { get => 0f; set { } }
        public float timeSamples { get => 0; set { } }
        public bool isPlaying => false;
        public bool isVirtual => false;
        public int priority { get => 128; set { } }
        public float panStereo { get => 0f; set { } }
        public float spatialBlend { get => 0f; set { } }
        public float spatialize { get => 0f; set { } }
        public float dopplerLevel { get => 1f; set { } }
        public float minDistance { get => 1f; set { } }
        public float maxDistance { get => 500f; set { } }
        public AudioRolloffMode rolloffMode { get => default; set { } }
        public AudioVelocityUpdateMode velocityUpdateMode { get => default; set { } }
        public Audio.AudioMixerGroup outputAudioMixerGroup { get => null; set { } }
        public void Play() { }
        public void PlayDelayed(float delay) { }
        public void PlayOneShot(AudioClip clip) { }
        public void PlayOneShot(AudioClip clip, float volumeScale) { }
        public void PlayScheduled(double time) { }
        public void Stop() { }
        public void Pause() { }
        public void UnPause() { }
    }

    public class AudioMixer : Object
    {
        public Audio.AudioMixerGroup FindMatchingGroups(string subPath) => null;
        public Audio.AudioMixerGroup[] FindMatchingGroupsArray(string subPath) => null;
        public bool GetFloat(string name, out float value) { value = 0f; return false; }
        public bool SetFloat(string name, float value) => false;
        public AudioMixerSnapshot FindSnapshot(string name) => null;
        public void TransitionToSnapshots(AudioMixerSnapshot[] snapshots, float[] weights, float timeToReach) { }
    }

    public class AudioMixerSnapshot : Object { }

    public class AudioReverbZone : Behaviour { }

    public static class AudioSettings
    {
        public static float dspTime => 0f;
        public static int outputSampleRate => 0;
    }

    public class Font : Object
    {
        public int fontSize { get => 0; set { } }
        public string[] fontNames => null;
        public Material material => null;
        public static Font CreateDynamicFontFromOSFont(string fontname, int size) => null;
        public bool HasCharacter(char c) => false;
        public int characterInfo => 0;
    }

    public class RuntimeAnimatorController : Object
    {
        public AnimationClip[] animationClips => null;
    }

    public class AnimationClip : Object
    {
        public float length => 0f;
        public float frameRate { get => 0f; set { } }
    }

    public class AnimatorOverrideController : RuntimeAnimatorController
    {
        public AnimatorOverrideController(RuntimeAnimatorController controller) { }
        public RuntimeAnimatorController runtimeAnimatorController { get => null; set { } }
    }

    public class Animator : Behaviour
    {
        public RuntimeAnimatorController runtimeAnimatorController { get => null; set { } }
        public bool applyRootMotion { get => false; set { } }
        public bool cullingMode { get => false; set { } }
        public float speed { get => 1f; set { } }
        public Vector3 deltaPosition => default;
        public Quaternion deltaRotation => default;
        public Vector3 rootPosition { get => default; set { } }
        public Quaternion rootRotation { get => default; set { } }
        public float GetFloat(string name) => 0f;
        public float GetFloat(int id) => 0f;
        public void SetFloat(string name, float value) { }
        public void SetFloat(int id, float value) { }
        public void SetFloat(string name, float value, float dampTime, float deltaTime) { }
        public void SetFloat(int id, float value, float dampTime, float deltaTime) { }
        public bool GetBool(string name) => false;
        public bool GetBool(int id) => false;
        public void SetBool(string name, bool value) { }
        public void SetBool(int id, bool value) { }
        public int GetInteger(string name) => 0;
        public int GetInteger(int id) => 0;
        public void SetInteger(string name, int value) { }
        public void SetInteger(int id, int value) { }
        public void SetTrigger(string name) { }
        public void SetTrigger(int id) { }
        public void ResetTrigger(string name) { }
        public void ResetTrigger(int id) { }
        public void Play(string stateName) { }
        public void Play(string stateName, int layer, float normalizedTime) { }
        public void Play(int stateNameHash) { }
        public void Rebind() { }
        public void Update(float deltaTime) { }
        public bool IsInLayerCount(int layerIndex) => false;
        public bool IsParameterControlledByCurve(int id) => false;
        public float GetLayerWeight(int layerIndex) => 0f;
        public void SetLayerWeight(int layerIndex, float weight) { }
        public int GetLayerIndex(string layerName) => 0;
        public int layerCount => 0;
        public static int StringToHash(string name) => 0;
    }

    public class AnimatorStateInfo
    {
        public string name => "";
        public int shortNameHash => 0;
        public int fullPathHash => 0;
        public float normalizedTime => 0f;
        public float length => 0f;
    }

    public class StateMachineBehaviour : ScriptableObject
    {
        public virtual void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public virtual void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public virtual void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    }

    public struct GradientColorKey
    {
        public GradientColorKey(Color color, float time) { }
        public Color color { get => default; set { } }
        public float time { get => 0f; set { } }
    }

    public struct GradientAlphaKey
    {
        public GradientAlphaKey(float alpha, float time) { }
        public float alpha { get => 0f; set { } }
        public float time { get => 0f; set { } }
    }

    public class Gradient
    {
        public GradientColorKey[] colorKeys { get => null; set { } }
        public GradientAlphaKey[] alphaKeys { get => null; set { } }
        public GradientMode mode { get => default; set { } }
        public Color Evaluate(float time) => default;
        public void SetKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys) { }
    }

    public enum GradientMode { Blend, Fixed }

    public class RectOffset
    {
        public RectOffset() { }
        public RectOffset(int left, int right, int top, int bottom) { }
        public int left { get => 0; set { } }
        public int right { get => 0; set { } }
        public int top { get => 0; set { } }
        public int bottom { get => 0; set { } }
        public int horizontal => 0;
        public int vertical => 0;
    }

    public class TextGenerationSettings { }

    // Attributes
    public class SerializeField : Attribute { }
    public class SerializeReference : Attribute { }
    public class HideInInspector : Attribute { }
    public class TextAreaAttribute : Attribute
    {
        public TextAreaAttribute() { }
        public TextAreaAttribute(int minLines, int maxLines) { }
    }
    public class HeaderAttribute : Attribute { public HeaderAttribute(string header) { } }
    public class TooltipAttribute : Attribute { public TooltipAttribute(string tooltip) { } }
    public class RangeAttribute : Attribute { public RangeAttribute(float min, float max) { } }
    public class SpaceAttribute : Attribute { public SpaceAttribute() { } public SpaceAttribute(float height) { } }
    public class ContextMenu : Attribute { public ContextMenu(string itemName) { } }
    public class RequireComponent : Attribute
    {
        public RequireComponent(Type requiredComponent) { }
        public RequireComponent(Type requiredComponent, Type requiredComponent2) { }
        public RequireComponent(Type requiredComponent, Type requiredComponent2, Type requiredComponent3) { }
    }
    public class AddComponentMenu : Attribute { public AddComponentMenu(string menuName) { } }
    public class ExecuteInEditMode : Attribute { }
    public class ExecuteAlways : Attribute { }
    public class DisallowMultipleComponent : Attribute { }
    public class SelectionBaseAttribute : Attribute { }
    public class RuntimeInitializeOnLoadMethodAttribute : Attribute
    {
        public RuntimeInitializeOnLoadMethodAttribute() { }
        public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType loadType) { }
    }
    public class CreateAssetMenuAttribute : Attribute
    {
        public string menuName { get; set; }
        public string fileName { get; set; }
        public int order { get; set; }
    }
    public class MulticastDelegate { }

    public static class Resources
    {
        public static T Load<T>(string path) where T : Object => default;
        public static Object Load(string path) => null;
        public static Object Load(string path, Type type) => null;
        public static T[] LoadAll<T>(string path) where T : Object => null;
        public static T GetBuiltinResource<T>(string path) where T : Object => default;
        public static Object[] FindObjectsOfTypeAll(Type type) => null;
        public static T[] FindObjectsOfTypeAll<T>() where T : Object => null;
        public static void UnloadUnusedAssets() { }
    }

    public class ScriptableObject : Object
    {
        public static ScriptableObject CreateInstance(Type type) => null;
        public static T CreateInstance<T>() where T : ScriptableObject => default;
    }

    public static class GL { }

    public static class Graphics
    {
        public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation) { }
        public static void Blit(Texture source, RenderTexture dest) { }
    }

    public class ComputeBuffer : IDisposable
    {
        public ComputeBuffer(int count, int stride) { }
        public void Dispose() { }
        public void Release() { }
    }

    public class SystemInfo
    {
        public static string deviceModel => "";
        public static string operatingSystem => "";
        public static int systemMemorySize => 0;
        public static int graphicsMemorySize => 0;
        public static string processorType => "";
        public static int processorCount => 0;
        public static string graphicsDeviceName => "";
        public static bool supportsInstancing => false;
    }

    public enum Space3D { }
}

// ============================== UnityEngine.Audio ==============================
namespace UnityEngine.Audio
{
    public class AudioMixerGroup : UnityEngine.Object
    {
        public UnityEngine.AudioMixer audioMixer => null;
    }

    public class AudioMixerGroupArray { }
}

// ============================== UnityEngine.Rendering ==============================
namespace UnityEngine.Rendering
{
    public enum AmbientMode { Skybox, Trilight, Flat, Custom }
    public enum ShadowCastingMode { Off, On, TwoSided, ShadowsOnly }
    public enum DefaultFormat { LDR, HDR }
    public enum TextureDimension { Unknown, None, Any, Tex2D, Tex3D, Cube, Tex2DArray, CubeArray }
    public enum ColorSpace { Uninitialized = -1, Gamma = 0, Linear = 1 }
    public class CommandBuffer : System.IDisposable
    {
        public string name { get; set; }
        public void Dispose() { }
        public void Clear() { }
    }
}

// ============================== UnityEngine.SceneManagement ==============================
namespace UnityEngine.SceneManagement
{
    public struct Scene
    {
        public string name => "";
        public string path => "";
        public int buildIndex => 0;
        public int handle => 0;
        public bool isLoaded => false;
        public bool IsValid() => false;
        public int rootCount => 0;
        public GameObject[] GetRootGameObjects() => null;
        public void GetRootGameObjects(System.Collections.Generic.List<GameObject> rootGameObjects) { }
    }

    public enum LoadSceneMode { Single, Additive }
    public enum LocalPhysicsMode { None, Physics2D, Physics3D }

    public static class SceneManager
    {
        public static Scene GetActiveScene() => default;
        public static Scene GetSceneAt(int index) => default;
        public static int sceneCount => 0;
        public static int sceneCountInBuildSettings => 0;
        public static void LoadScene(string sceneName) { }
        public static void LoadScene(string sceneName, LoadSceneMode mode) { }
        public static void LoadScene(int buildIndex) { }
        public static void LoadScene(int buildIndex, LoadSceneMode mode) { }
        public static AsyncOperation LoadSceneAsync(string sceneName) => null;
        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode) => null;
        public static AsyncOperation LoadSceneAsync(int buildIndex) => null;
        public static bool SetActiveScene(Scene scene) => false;
        public static void MoveGameObjectToScene(GameObject go, Scene scene) { }
    }
}

// ============================== UnityEngine.EventSystems ==============================
namespace UnityEngine.EventSystems
{
    public class UIBehaviour : UnityEngine.MonoBehaviour { }

    public class EventSystem : UIBehaviour
    {
        public static EventSystem current { get => null; set { } }
        public GameObject currentSelectedGameObject => null;
        public bool IsPointerOverGameObject() => false;
        public bool IsPointerOverGameObject(int pointerId) => false;
        public void SetSelectedGameObject(GameObject selected) { }
        public void SetSelectedGameObject(GameObject selected, BaseEventData pointer) { }
    }

    public class BaseInputModule : UIBehaviour { }
    public class PointerInputModule : BaseInputModule { }
    public class StandaloneInputModule : PointerInputModule
    {
        public string horizontalAxis { get => ""; set { } }
        public string verticalAxis { get => ""; set { } }
        public string submitButton { get => ""; set { } }
        public string cancelButton { get => ""; set { } }
        public float inputActionsPerSecond { get => 0f; set { } }
        public float repeatDelay { get => 0f; set { } }
        public bool forceModuleActive { get => false; set { } }
    }
    public class BaseEventData { }
    public class PointerEventData : BaseEventData
    {
        public UnityEngine.Vector2 position => default;
        public UnityEngine.Vector2 delta => default;
        public UnityEngine.Vector2 scrollDelta => default;
    }
}

// ============================== UnityEngine.UI ==============================
namespace UnityEngine.UI
{
    public class Graphic : UnityEngine.EventSystems.UIBehaviour
    {
        public Color color { get => default; set { } }
        public Material material { get => default; set { } }
        public bool raycastTarget { get => false; set { } }
        public RectTransform rectTransform => null;
        public Canvas canvas => null;
        public bool enabled { get => false; set { } }
        public virtual void SetAllDirty() { }
        public virtual void SetLayoutDirty() { }
        public virtual void SetVerticesDirty() { }
        public virtual void SetMaterialDirty() { }
    }

    public class MaskableGraphic : Graphic { }

    public enum TextAnchor { UpperLeft, UpperCenter, UpperRight, MiddleLeft, MiddleCenter, MiddleRight, LowerLeft, LowerCenter, LowerRight }
    public enum FontStyle { Normal, Bold, Italic, BoldAndItalic }
    public enum HorizontalWrapMode { Wrap, Overflow }
    public enum VerticalWrapMode { Truncate, Overflow }

    public class Text : MaskableGraphic
    {
        public string text { get => ""; set { } }
        public Font font { get => null; set { } }
        public int fontSize { get => 14; set { } }
        public FontStyle fontStyle { get => default; set { } }
        public TextAnchor alignment { get => default; set { } }
        public HorizontalWrapMode horizontalOverflow { get => default; set { } }
        public VerticalWrapMode verticalOverflow { get => default; set { } }
        public bool resizeTextForBestFit { get => false; set { } }
        public int resizeTextMinSize { get => 0; set { } }
        public int resizeTextMaxSize { get => 0; set { } }
        public float lineSpacing { get => 1f; set { } }
        public float preferredWidth => 0f;
        public float preferredHeight => 0f;
        public float minWidth => 0f;
        public float minHeight => 0f;
        public bool supportRichText { get => false; set { } }
        public float pixelsPerUnit => 0f;
    }

    public class Image : MaskableGraphic
    {
        public enum Type { Simple, Sliced, Tiled, Filled }
        public enum FillMethod { Horizontal, Vertical, Radial90, Radial180, Radial360 }
        public Sprite sprite { get => null; set { } }
        public Type type { get => default; set { } }
        public float fillAmount { get => 1f; set { } }
        public FillMethod fillMethod { get => default; set { } }
        public bool fillCenter { get => false; set { } }
        public bool preserveAspect { get => false; set { } }
        public float pixelsPerUnitMultiplier { get => 1f; set { } }
        public float alphaHitTestMinimumThreshold { get => 0f; set { } }
        public override void SetAllDirty() { }
    }

    public class RawImage : MaskableGraphic
    {
        public Texture texture { get => null; set { } }
        public Rect uvRect { get => default; set { } }
    }

    public class Selectable : UnityEngine.EventSystems.UIBehaviour
    {
        public enum Transition { None, ColorTint, SpriteSwap, Animation }
        public bool interactable { get => false; set { } }
        public Transition transition { get => default; set { } }
        public Image image { get => null; set { } }
        public Graphic targetGraphic { get => null; set { } }
        public ColorBlock colors { get => default; set { } }
        public bool IsInteractable() => false;
        public virtual void Select() { }
    }

    public class Button : Selectable
    {
        public class ButtonClickedEvent : UnityEngine.Events.UnityEvent { }
        public ButtonClickedEvent onClick => null;
        public bool interactable { get => false; set { } }
    }

    public class Toggle : Selectable
    {
        public class ToggleEvent : UnityEngine.Events.UnityEvent<bool> { }
        public bool isOn { get => false; set { } }
        public ToggleEvent onValueChanged => null;
        public Graphic graphic { get => null; set { } }
        public void SetIsOnWithoutNotify(bool value) { }
    }

    public class Slider : Selectable
    {
        public enum Direction { LeftToRight, RightToLeft, BottomToTop, TopToBottom }
        public class SliderEvent : UnityEngine.Events.UnityEvent<float> { }
        public float value { get => 0f; set { } }
        public float minValue { get => 0f; set { } }
        public float maxValue { get => 1f; set { } }
        public bool wholeNumbers { get => false; set { } }
        public SliderEvent onValueChanged => null;
        public Direction direction { get => default; set { } }
        public RectTransform fillRect { get => null; set { } }
        public RectTransform handleRect { get => null; set { } }
        public void SetValueWithoutNotify(float input) { }
    }

    public class Scrollbar : Selectable
    {
        public class ScrollbarEvent : UnityEngine.Events.UnityEvent<float> { }
        public float value { get => 0f; set { } }
        public ScrollbarEvent onValueChanged => null;
    }

    public class Dropdown : Selectable
    {
        public class DropdownEvent : UnityEngine.Events.UnityEvent<int> { }
        public class OptionData
        {
            public OptionData() { }
            public OptionData(string text) { }
            public string text { get => ""; set { } }
            public Sprite image { get => null; set { } }
        }
        public class OptionDataList
        {
            public List<OptionData> options => null;
        }
        public int value { get => 0; set { } }
        public List<OptionData> options { get => null; set { } }
        public DropdownEvent onValueChanged => null;
        public Text captionText { get => null; set { } }
        public Image captionImage { get => null; set { } }
        public void SetValueWithoutNotify(int input) { }
        public void ClearOptions() { }
        public void AddOptions(List<string> options) { }
        public void AddOptions(List<OptionData> options) { }
        public void AddOptions(string[] options) { }
        public void RefreshShownValue() { }
    }

    public class InputField : Selectable
    {
        public string text { get => ""; set { } }
        public string placeholder { get => ""; set { } }
    }

    public class ScrollRect : UnityEngine.EventSystems.UIBehaviour
    {
        public RectTransform content { get => null; set { } }
        public bool horizontal { get => false; set { } }
        public bool vertical { get => false; set { } }
        public Vector2 normalizedPosition { get => default; set { } }
        public void StopMovement() { }
    }

    public class LayoutGroup : UnityEngine.EventSystems.UIBehaviour
    {
        public RectOffset padding { get => null; set { } }
        public TextAnchor childAlignment { get => default; set { } }
    }

    public class HorizontalOrVerticalLayoutGroup : LayoutGroup
    {
        public float spacing { get => 0f; set { } }
        public bool childForceExpandWidth { get => false; set { } }
        public bool childForceExpandHeight { get => false; set { } }
        public bool childControlWidth { get => false; set { } }
        public bool childControlHeight { get => false; set { } }
        public bool childScaleWidth { get => false; set { } }
        public bool childScaleHeight { get => false; set { } }
    }

    public class HorizontalLayoutGroup : HorizontalOrVerticalLayoutGroup { }
    public class VerticalLayoutGroup : HorizontalOrVerticalLayoutGroup { }

    public class GridLayoutGroup : LayoutGroup
    {
        public Vector2 cellSize { get => default; set { } }
        public Vector2 spacing { get => default; set { } }
        public int constraintCount { get => 0; set { } }
    }

    public class ContentSizeFitter : UnityEngine.EventSystems.UIBehaviour
    {
        public enum FitMode { Unconstrained, MinSize, PreferredSize }
        public FitMode horizontalFit { get => default; set { } }
        public FitMode verticalFit { get => default; set { } }
    }

    public class LayoutElement : UnityEngine.EventSystems.UIBehaviour
    {
        public float minWidth { get => -1f; set { } }
        public float minHeight { get => -1f; set { } }
        public float preferredWidth { get => -1f; set { } }
        public float preferredHeight { get => -1f; set { } }
        public float flexibleWidth { get => -1f; set { } }
        public float flexibleHeight { get => -1f; set { } }
        public int layoutPriority { get => 0; set { } }
        public bool ignoreLayout { get => false; set { } }
    }

    public class CanvasScaler : UnityEngine.EventSystems.UIBehaviour
    {
        public enum ScaleMode { ConstantPixelSize, ScaleWithScreenSize, ConstantPhysicalSize }
        public ScaleMode uiScaleMode { get => default; set { } }
        public Vector2 referenceResolution { get => default; set { } }
        public float matchWidthOrHeight { get => 0.5f; set { } }
        public float scaleFactor { get => 1f; set { } }
    }

    public class GraphicRaycaster : UnityEngine.EventSystems.BaseInputModule { }

    public class Shadow : UnityEngine.EventSystems.UIBehaviour
    {
        public Color effectColor { get => default; set { } }
        public Vector2 effectDistance { get => default; set { } }
    }

    public class Outline : Shadow { }

    public class RectMask2D : UnityEngine.EventSystems.UIBehaviour { }

    public class ClipperRegistry { public static ClipperRegistry instance => null; }
}

// ============================== UnityEngine.Events ==============================
namespace UnityEngine.Events
{
    public delegate void UnityAction();
    public delegate void UnityAction<T0>(T0 arg0);
    public delegate void UnityAction<T0, T1>(T0 arg0, T1 arg1);

    public class UnityEventBase
    {
        public int GetPersistentEventCount() => 0;
        public void RemoveAllListeners() { }
    }

    public class UnityEvent : UnityEventBase
    {
        public void AddListener(UnityAction call) { }
        public void RemoveListener(UnityAction call) { }
        public void Invoke() { }
    }

    public class UnityEvent<T0> : UnityEventBase
    {
        public void AddListener(UnityAction<T0> call) { }
        public void RemoveListener(UnityAction<T0> call) { }
        public void Invoke(T0 arg0) { }
    }

    public class UnityEvent<T0, T1> : UnityEventBase
    {
        public void AddListener(UnityAction<T0, T1> call) { }
        public void RemoveListener(UnityAction<T0, T1> call) { }
        public void Invoke(T0 arg0, T1 arg1) { }
    }
}

// ============================== UnityEditor ==============================
namespace UnityEditor
{
    using UnityEngine;

    public class MenuItem : System.Attribute
    {
        public MenuItem(string itemName) { }
        public MenuItem(string itemName, bool isValidateFunction) { }
        public MenuItem(string itemName, bool isValidateFunction, int priority) { }
    }

    public class CustomEditor : System.Attribute
    {
        public CustomEditor(Type inspectedType) { }
        public CustomEditor(Type inspectedType, bool editorForChildClasses) { }
    }

    public static class EditorUtility
    {
        public static void SetDirty(UnityEngine.Object target) { }
        public static void CopySerialized(UnityEngine.Object source, UnityEngine.Object dest) { }
        public static bool DisplayCancelableProgressBar(string title, string info, float progress) => false;
        public static void DisplayProgressBar(string title, string info, float progress) { }
        public static void ClearProgressBar() { }
        public static bool DisplayDialog(string title, string message, string ok) => false;
        public static bool DisplayDialog(string title, string message, string ok, string cancel) => false;
        public static string OpenFilePanel(string title, string directory, string extension) => "";
        public static string SaveFilePanel(string title, string directory, string defaultName, string extension) => "";
        public static bool IsPersistent(UnityEngine.Object target) => false;
        public static void FocusProjectWindow() { }
    }

    public static class AssetDatabase
    {
        public static UnityEngine.Object LoadAssetAtPath(string assetPath, Type type) => null;
        public static T LoadAssetAtPath<T>(string assetPath) where T : UnityEngine.Object => default;
        public static string GenerateUniqueAssetPath(string path) => "";
        public static string CreateFolder(string parentFolder, string newFolderName) => "";
        public static bool IsValidFolder(string path) => false;
        public static bool DeleteAsset(string path) => false;
        public static bool CopyAsset(string path, string newPath) => false;
        public static string MoveAsset(string oldPath, string newPath) => "";
        public static string RenameAsset(string pathName, string newName) => "";
        public static void CreateAsset(UnityEngine.Object asset, string path) { }
        public static void AddObjectToAsset(UnityEngine.Object objectToAdd, UnityEngine.Object assetObject) { }
        public static void SaveAssets() { }
        public static void Refresh() { }
        public static void Refresh(ImportAssetOptions options) { }
        public static string[] FindAssets(string filter) => null;
        public static string[] FindAssets(string filter, string[] searchInFolders) => null;
        public static string GUIDToAssetPath(string guid) => "";
        public static string AssetPathToGUID(string path) => "";
        public static void StartAssetEditing() { }
        public static void StopAssetEditing() { }
        public static void ImportAsset(string path) { }
        public static void ImportAsset(string path, ImportAssetOptions options) { }
    }

    public enum ImportAssetOptions { Default = 0, ForceUpdate = 1, ForceSynchronousImport = 8, ImportRecursive = 256, DontDownloadFromCacheServer = 8192, ForceUncompressedImport = 16384 }

    public static class Selection
    {
        public static UnityEngine.Object activeObject { get => null; set { } }
        public static GameObject activeGameObject { get => null; set { } }
        public static Transform activeTransform { get => null; set { } }
        public static UnityEngine.Object[] objects => null;
    }

    public static class EditorApplication
    {
        public delegate void CallbackFunction();
        public static CallbackFunction delayCall;
        public static CallbackFunction update;
        public static CallbackFunction hierarchyChanged;
        public static CallbackFunction projectChanged;
        public static bool isPlaying { get => false; set { } }
        public static bool isCompiling => false;
        public static bool isPaused { get => false; set { } }
        public static bool isPlayingOrWillChangePlaymode => false;
        public static double time => 0d;
        public static void ExecuteMenuItem(string menuItemPath) { }
        public static void Beep() { }
        public static void Exit(int returnValue) { }
        public static bool SaveCurrentSceneIfUserWantsTo() => false;
    }

    public static class EditorSceneManager
    {
        public static UnityEngine.SceneManagement.Scene NewScene(NewSceneSetup setup, NewSceneMode mode) => default;
        public static UnityEngine.SceneManagement.Scene NewScene(NewSceneSetup setup, NewSceneMode mode, NewSceneMode modeOverride) => default;
        public static bool SaveScene(UnityEngine.SceneManagement.Scene scene) => false;
        public static bool SaveScene(UnityEngine.SceneManagement.Scene scene, string dstScenePath) => false;
        public static bool SaveScene(UnityEngine.SceneManagement.Scene scene, string dstScenePath, bool saveAsCopy) => false;
        public static bool SaveCurrentModifiedScenesIfUserWantsTo() => false;
        public static UnityEngine.SceneManagement.Scene OpenScene(string scenePath) => default;
        public static UnityEngine.SceneManagement.Scene OpenScene(string scenePath, OpenSceneMode mode) => default;
        public static UnityEngine.SceneManagement.Scene GetActiveScene() => default;
        public static bool MarkSceneDirty(UnityEngine.SceneManagement.Scene scene) => false;
    }

    public enum NewSceneSetup { EmptyScene, DefaultGameObjects }
    public enum NewSceneMode { Single, Additive }
    public enum OpenSceneMode { Single, Additive, AdditiveWithoutLoading }

    public static class EditorBuildSettings
    {
        public static EditorBuildSettingsScene[] scenes { get => null; set { } }
        public static string activeScenePath { get => ""; set { } }
        public static int GetActiveSceneIndex(string scenePath) => 0;
    }

    public class EditorBuildSettingsScene
    {
        public EditorBuildSettingsScene() { }
        public EditorBuildSettingsScene(string path, bool enabled) { }
        public string path { get => ""; set { } }
        public bool enabled { get => false; set { } }
        public int guid => 0;
    }

    [Flags]
    public enum BuildOptions
    {
        None = 0,
        Development = 1,
        AllowDebugging = 2,
        AutoRunPlayer = 4,
        ShowBuiltPlayer = 8,
        BuildAdditionalStreamedScenes = 16,
        AcceptExternalModificationsToPlayer = 32,
        InstallInBuildFolder = 64,
        WebPlayerOfflineDeployment = 128,
        ConnectWithProfiler = 256,
        EnableHeadlessMode = 512,
        BuildScriptsOnly = 1024,
        PatchPackage = 2048,
        Il2CPPCodeGeneration = 4096,
        StrictMode = 8192,
        IncludeTestAssemblies = 16384,
        SymlinkSources = 32768,
        ForceEnableAssertions = 131072,
        EditorCompatibilityMode = 262144,
        ExplicitNullChecks = 524288,
        ExplicitDivideByZeroChecks = 1048576,
        ExplicitArrayBoundsChecks = 2097152,
        ForceOptimizeScriptCompilation = 8388608,
        ComputeCRC = 16777216,
        DisableManagedStripping = 33554432,
        DisableTextureBasisCompression = 67108864,
        EnableDeepProfilingSupport = 134217728,
        DetailedBuildReport = 268435456
    }

    public enum BuildTarget
    {
        StandaloneOSX = 2, StandaloneWindows = 5, iOS = 9, PS4 = 31, Android = 13, StandaloneWindows64 = 19,
        WebGL = 20, WSAPlayer = 21, StandaloneLinux64 = 24, Switch = 38, XboxOne = 27, GameCoreXboxOne = 33,
        GameCoreXboxSeries = 34, PS5 = 36, EmbeddedLinux = 39, QNX = 40, visionOS = 43, NoTarget = -1
    }

    public enum BuildTargetGroup
    {
        Unknown = 0, Standalone = 1, iPhone = 4, iOS = 4, Android = 7, WebGL = 13, WSA = 14,
        PS4 = 19, XboxOne = 21, tvOS = 22, Switch = 25, Lumin = 26, Stadia = 27, CloudRendering = 28,
        GameCoreXboxSeries = 30, GameCoreXboxOne = 31, PS5 = 32, QNX = 33, EmbeddedLinux = 34, visionOS = 35
    }

    public enum ScriptingImplementation { Mono2x = 0, IL2CPP = 1, WinRTDotNET = 2 }

    [Flags]
    public enum AndroidArchitecture { None = 0, ARMv7 = 1, ARM64 = 2, X86 = 4, X86_64 = 8, All = 15 }

    [Flags]
    public enum AndroidTargetArchitectures { None = 0, ARMv7 = 1, ARM64 = 2, X86 = 4, X86_64 = 8, All = 15 }

    public enum AndroidSdkVersions
    {
        AndroidApiLevelAuto = 0,
        AndroidApiLevel22 = 22,
        AndroidApiLevel23 = 23,
        AndroidApiLevel24 = 24,
        AndroidApiLevel25 = 25,
        AndroidApiLevel26 = 26,
        AndroidApiLevel27 = 27,
        AndroidApiLevel28 = 28,
        AndroidApiLevel29 = 29,
        AndroidApiLevel30 = 30,
        AndroidApiLevel31 = 31,
        AndroidApiLevel32 = 32,
        AndroidApiLevel33 = 33,
        AndroidApiLevel34 = 34,
        AndroidApiLevel35 = 35
    }

    public enum AndroidBuildSystem { Auto, Gradle, Internal, VisualStudio }
    public enum AndroidBuildType { Debug, Development, Release }
    public enum AndroidMinification { Disabled, Proguard, Gradle }
    public enum UIOrientation { Portrait, PortraitUpsideDown, LandscapeLeft, LandscapeRight, AutoRotation }
    public enum ManagedStrippingLevel { Disabled, Low, Medium, High }
    public enum InputHandling { InputManager, InputSystemPackage, Both }
    public enum LightmapEncodingQuality { Low = 0, Normal = 1, High = 2 }
    public enum TextureCompressionFormat { Automatic = 0, DXT = 1, ETC2 = 2, ASTC = 3 }
    public enum GraphicsDeviceType { Direct3D11 = 2, Direct3D12 = 18, OpenGLES2 = 8, OpenGLES3 = 11, Vulkan = 21, Metal = 16, OpenGLCore = 17, Null = 4 }

    public static class PlayerSettings
    {
        public static string companyName { get => ""; set { } }
        public static string productName { get => ""; set { } }
        public static string defaultScreenWidth { get => ""; set { } }
        public static int defaultScreenWidthNative { get => 0; set { } }
        public static int defaultScreenHeightNative { get => 0; set { } }
        public static string bundleVersion { get => ""; set { } }
        public static int AndroidBundleVersionCode { get => 0; set { } }
        public static bool stripEngineCode { get => false; set { } }
        public static UnityEngine.Rendering.ColorSpace colorSpace { get => default; set { } }
        public static bool graphicsJobs { get => false; set { } }
        public static void SetMobileMTRendering(BuildTargetGroup targetGroup, bool enable) { }
        public static UIOrientation defaultInterfaceOrientation { get => default; set { } }
        public static bool allowedAutorotateToPortrait { get => false; set { } }
        public static bool allowedAutorotateToPortraitUpsideDown { get => false; set { } }
        public static bool allowedAutorotateToLandscapeLeft { get => false; set { } }
        public static bool allowedAutorotateToLandscapeRight { get => false; set { } }
        public static bool muteOtherAudioSources { get => false; set { } }
        public static bool prepareIOSForRecording { get => false; set { } }
        public static int GetScriptingBackend(BuildTargetGroup targetGroup) => 0;
        public static void SetScriptingBackend(BuildTargetGroup targetGroup, ScriptingImplementation backend) { }
        public static ScriptingImplementation GetScriptingBackendForGroup(BuildTargetGroup targetGroup) => default;
        public static void SetScriptingBackendForGroup(BuildTargetGroup targetGroup, ScriptingImplementation backend) { }
        public static string GetApplicationIdentifier(BuildTargetGroup targetGroup) => "";
        public static void SetApplicationIdentifier(BuildTargetGroup targetGroup, string identifier) { }
        public static int GetGraphicsAPIs(BuildTarget target) => 0;
        public static void SetGraphicsAPIs(BuildTarget target, GraphicsDeviceType[] graphicsAPIs) { }
        public static GraphicsDeviceType[] GetGraphicsAPIsForBuildTarget(BuildTarget target) => null;
        public static void SetGraphicsAPIsForBuildTarget(BuildTarget target, GraphicsDeviceType[] graphicsAPIs) { }
        public static string GetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup) => "";
        public static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup, string defines) { }
        public static void SetScriptingDefineSymbols(BuildTargetGroup targetGroup, string defines) { }
        public static string GetScriptingDefineSymbols(BuildTargetGroup targetGroup) => "";
        public static int GetArchitecture(BuildTargetGroup targetGroup) => 0;
        public static void SetArchitecture(BuildTargetGroup targetGroup, int architecture) { }
        public static ManagedStrippingLevel GetManagedStrippingLevel(BuildTargetGroup targetGroup) => default;
        public static void SetManagedStrippingLevel(BuildTargetGroup targetGroup, ManagedStrippingLevel level) { }
        public static bool GetUseDefaultGraphicsAPIs(BuildTarget platform) => false;
        public static void SetUseDefaultGraphicsAPIs(BuildTarget platform, bool automatic) { }
        public static int GetPreloadedAssetsCount() => 0;

        public static class SplashScreen
        {
            public static bool show { get => false; set { } }
            public static bool showUnityLogo { get => false; set { } }
            public static SplashScreenLogo[] logos { get => null; set { } }
            public static int logoStyle { get => 0; set { } }
            public static int animationMode { get => 0; set { } }
            public static int drawMode { get => 0; set { } }
            public static float overlayOpacity { get => 0f; set { } }
            public static Sprite background { get => null; set { } }
            public static Color backgroundColor { get => default; set { } }
            public static Sprite backgroundPortrait { get => null; set { } }
        }

        public static class Android
        {
            public static AndroidSdkVersions minSdkVersion { get => default; set { } }
            public static AndroidSdkVersions targetSdkVersion { get => default; set { } }
            public static AndroidArchitecture targetArchitectures { get => default; set { } }
            public static AndroidBuildType buildType { get => default; set { } }
            public static AndroidBuildSystem buildSystem { get => default; set { } }
            public static AndroidMinification minifyWithR8 { get => default; set { } }
            public static AndroidMinification minifyWithProguard { get => default; set { } }
            public static bool splitApplicationBinary { get => false; set { } }
            public static bool useCustomKeystore { get => false; set { } }
            public static string keystoreName { get => ""; set { } }
            public static string keystorePass { get => ""; set { } }
            public static string keyaliasName { get => ""; set { } }
            public static string keyaliasPass { get => ""; set { } }
            public static int bundleVersionCode { get => 1; set { } }
            public static bool forceInternetPermission { get => false; set { } }
            public static bool forceSDCardPermission { get => false; set { } }
            public static bool androidIsGame { get => false; set { } }
            public static bool androidTVCompatibility { get => false; set { } }
            public static int blitType { get => 0; set { } }
        }

        public static class iOS
        {
            public static string buildNumber { get => ""; set { } }
            public static bool accelerometerFrequency { get => false; set { } }
            public static int targetOSVersionString => 0;
        }
    }

    public class BuildPlayerOptions
    {
        public string locationPathName;
        public string[] scenes;
        public string assetBundleManifestPath;
        public BuildTarget target;
        public BuildTargetGroup targetGroup;
        public BuildOptions options;
        public int subtarget;
    }

    public static class BuildPipeline
    {
        public static Build.Reporting.BuildReport BuildPlayer(BuildPlayerOptions buildPlayerOptions) => null;
        public static Build.Reporting.BuildReport BuildPlayer(string[] levels, string locationPathName, BuildTarget target, BuildOptions options) => null;
        public static bool IsBuildTargetSupported(BuildTargetGroup targetGroup, BuildTarget target) => false;
        public static string GetBuildTargetName(BuildTarget target) => "";
    }

    public class BuildPlayerWindow
    {
        public static class DefaultBuildMethods
        {
            public static void BuildPlayer(BuildPlayerOptions options) { }
        }
        public static void RegisterBuildPlayerHandler(BuildPlayerHandler handler) { }
        public delegate void BuildPlayerHandler(BuildPlayerOptions options);
    }

    public class SerializedObject
    {
        public SerializedObject(UnityEngine.Object obj) { }
        public UnityEngine.Object targetObject => null;
        public UnityEngine.Object[] targetObjects => null;
        public void Update() { }
        public void ApplyModifiedProperties() { }
        public void ApplyModifiedPropertiesWithoutUndo() { }
        public void SetIsDifferentCacheDirty() { }
        public SerializedProperty FindProperty(string propertyPath) => null;
        public SerializedProperty GetIterator() => null;
    }

    public class SerializedProperty
    {
        public string name => "";
        public string displayName => "";
        public string propertyPath => "";
        public SerializedPropertyType propertyType => default;
        public bool boolValue { get => false; set { } }
        public int intValue { get => 0; set { } }
        public float floatValue { get => 0f; set { } }
        public string stringValue { get => ""; set { } }
        public Color colorValue { get => default; set { } }
        public Vector3 vector3Value { get => default; set { } }
        public UnityEngine.Object objectReferenceValue { get => null; set { } }
        public int objectReferenceInstanceIDValue { get => 0; set { } }
        public int enumValueIndex { get => 0; set { } }
        public SerializedProperty FindPropertyRelative(string relativePropertyPath) => null;
        public int arraySize { get => 0; set { } }
        public bool isArray => false;
        public bool hasVisibleChildren => false;
        public SerializedProperty GetArrayElementAtIndex(int index) => null;
        public bool Next(bool enterChildren) => false;
        public bool NextVisible(bool enterChildren) => false;
        public void ClearArray() { }
        public bool DeleteArrayElementAtIndex(int index) => false;
        public int InsertArrayElementAtIndex(int index) => 0;
    }

    public enum SerializedPropertyType { Generic = 0, Integer = 1, Boolean = 2, Float = 3, String = 4, Color = 5, ObjectReference = 6, LayerMask = 7, Enum = 8, Vector2 = 9, Vector3 = 10, Vector4 = 11, Rect = 12, ArraySize = 13, Character = 14, AnimationCurve = 15, Bounds = 16, Gradient = 17, Quaternion = 18, ExposedReference = 19, FixedBufferSize = 20, Vector2Int = 21, Vector3Int = 22, RectInt = 23, BoundsInt = 24, ManagedReference = 25 }

    public static class PrefabUtility
    {
        public static GameObject SaveAsPrefabAsset(GameObject root, string assetPath) => null;
        public static GameObject SaveAsPrefabAssetAndConnect(GameObject root, string assetPath, InteractionMode action) => null;
        public static UnityEngine.Object InstantiatePrefab(UnityEngine.Object target) => null;
        public static UnityEngine.Object InstantiatePrefab(UnityEngine.Object target, UnityEngine.SceneManagement.Scene destinationScene) => null;
        public static GameObject LoadPrefabContents(string path) => null;
        public static void UnloadPrefabContents(GameObject root) { }
        public static bool IsPartOfPrefabInstance(UnityEngine.Object componentOrGameObject) => false;
    }

    public enum InteractionMode { UserAction, AutomatedAction }

    public class Editor : ScriptableObject
    {
        public UnityEngine.Object target => null;
        public UnityEngine.Object[] targets => null;
        public SerializedObject serializedObject => null;
        public virtual void OnInspectorGUI() { }
        public virtual void OnPreviewGUI(Rect r, GUIStyle background) { }
        public void Repaint() { }
    }

    public class EditorWindow : ScriptableObject
    {
        public static T GetWindow<T>() where T : EditorWindow => default;
        public static T GetWindow<T>(string title) where T : EditorWindow => default;
        public void Show() { }
        public void Close() { }
        public void Repaint() { }
        public Rect position { get => default; set { } }
        public virtual void OnGUI() { }
    }

    public class PropertyDrawer { public virtual void OnGUI(Rect position, SerializedProperty property, GUIContent label) { } }
    public class CustomPropertyDrawer : System.Attribute { public CustomPropertyDrawer(Type type) { } public CustomPropertyDrawer(Type type, bool useForChildren) { } }

    public class EditorStyles
    {
        public static GUIStyle boldLabel => null;
        public static GUIStyle label => null;
        public static GUIStyle miniLabel => null;
        public static GUIStyle helpBox => null;
    }

    public class GUIStyle { public GUIStyle() { } public GUIStyle(GUIStyle other) { } public GUIStyle(string str) { } }
    public class GUIContent { public GUIContent() { } public GUIContent(string text) { } public GUIContent(string text, string tooltip) { } public string text { get; set; } }

    public static class GUILayout
    {
        public static void Label(string text) { }
        public static void Label(string text, params GUILayoutOption[] options) { }
        public static void Space(float pixels) { }
        public static bool Button(string text) => false;
        public static bool Toggle(bool value, string text) => false;
    }

    public static class EditorGUILayout
    {
        public static void LabelField(string label) { }
        public static void LabelField(string label, string label2) { }
        public static bool Toggle(string label, bool value) => false;
        public static int Popup(string label, int selectedIndex, string[] displayedOptions) => 0;
        public static float Slider(string label, float value, float leftValue, float rightValue) => 0f;
        public static void PropertyField(SerializedProperty property) { }
        public static void PropertyField(SerializedProperty property, bool includeChildren) { }
        public static void HelpBox(string message, MessageType type) { }
    }

    public class GUILayoutOption { }

    public class SplashScreenLogo
    {
        public Sprite logo { get => null; set { } }
        public float duration { get => 0f; set { } }
    }

    public static class GUI
    {
        public static Color color { get => default; set { } }
        public static Color backgroundColor { get => default; set { } }
        public static Color contentColor { get => default; set { } }
        public static void Label(Rect position, string text) { }
        public static bool Button(Rect position, string text) => false;
        public static void Box(Rect position, string text) { }
    }

    public enum MessageType { None, Info, Warning, Error }

    public static class Undo
    {
        public static void RecordObject(UnityEngine.Object objectToUndo, string name) { }
        public static void RegisterCreatedObjectUndo(UnityEngine.Object objectToUndo, string name) { }
    }

    public class InitializeOnLoadAttribute : System.Attribute { }
    public class InitializeOnLoadMethodAttribute : System.Attribute { }
    public class DidReloadScripts : System.Attribute { }

    namespace Build
    {
        public interface IPreprocessBuildWithReport
        {
            int callbackOrder { get; }
            void OnPreprocessBuild(Reporting.BuildReport report);
        }

        public interface IPostprocessBuildWithReport
        {
            int callbackOrder { get; }
            void OnPostprocessBuild(Reporting.BuildReport report);
        }

        public interface IProcessSceneWithReport
        {
            int callbackOrder { get; }
            void OnProcessScene(Reporting.BuildReport report, string scenePath);
        }
    }

    namespace Build.Reporting
    {
        public class BuildReport
        {
            public BuildSummary summary => default;
            public BuildFile[] files => null;
            public BuildStep[] steps => null;
            public int totalErrors => 0;
            public int totalWarnings => 0;
        }

        public struct BuildSummary
        {
            public BuildResult result => default;
            public BuildOptions options => default;
            public BuildTarget platform => default;
            public BuildTargetGroup platformGroup => default;
            public ulong totalSize => 0;
            public TimeSpan totalTime => default;
            public int totalErrors => 0;
            public int totalWarnings => 0;
        }

        public enum BuildResult { Unknown = 0, Succeeded = 1, Failed = 2, Cancelled = 3 }

        public struct BuildFile
        {
            public string path => "";
            public ulong size => 0;
            public string role => "";
        }

        public struct BuildStep
        {
            public string name => "";
            public int depth => 0;
            public float duration => 0f;
            public BuildStepMessage[] messages => null;
        }

        public struct BuildStepMessage
        {
            public string content => "";
            public LogType type => default;
        }
    }

    namespace SceneManagement
    {
        public static class PrefabStageUtility
        {
            public static GameObject GetCurrentPrefabStage() => null;
        }
    }
}

// ============================== UnityEditorInternal ==============================
namespace UnityEditorInternal
{
    public static class InternalEditorUtility
    {
        public static string[] tags => null;
        public static string[] layers => null;
        public static void AddTag(string tag) { }
        public static void AddLayer(int index, string layerName) { }
        public static bool TagExists(string tag) => false;
        public static bool LayerExists(string layerName) => false;
    }
}

// ============================== global aliases used by unity ==============================
namespace UnityEngine
{
    public enum LogType { Error, Assert, Warning, Log, Exception }
    public enum RuntimeInitializeMethodType { }
    public class ShadowCastingModeAlias { }
}
