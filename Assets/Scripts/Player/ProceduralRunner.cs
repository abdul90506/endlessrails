using System.Collections.Generic;
using UnityEngine;

namespace EndlessRails
{
    /// <summary>
    /// Built-in procedural humanoid rig used when no custom character model is
    /// assigned. Constructs a stylized mannequin from primitives at runtime and
    /// drives a code-based animation set (run cycle, lean, jump, slide, stumble,
    /// ragdoll, ponytail physics). When you assign a Mixamo/humanoid model via
    /// CharacterDefinition, this component is disabled automatically.
    /// </summary>
    public class ProceduralRunner : MonoBehaviour
    {
        [Header("Build Options")]
        [SerializeField] private CharacterDefinition appearance;
        [SerializeField] private bool buildOnAwake = true;
        [SerializeField] private float bodyScale = 1f;

        // Rig bones (transforms of generated parts).
        private Transform _root;
        private Transform _hips;
        private Transform _torso;
        private Transform _head;
        private Transform _upperArmL, _lowerArmL, _handL;
        private Transform _upperArmR, _lowerArmR, _handR;
        private Transform _upperLegL, _lowerLegL, _footL;
        private Transform _upperLegR, _lowerLegR, _footR;
        private Transform _ponytail;
        private readonly List<Rigidbody> _ragdollBodies = new List<Rigidbody>();

        // Animation state.
        private float _locomotionSpeed01 = 0.5f;
        private float _lean;
        private bool _airborne;
        private bool _sliding;
        private bool _jetpack;
        private bool _ragdoll;
        private float _jumpPoseTimer;
        private float _stumblePoseTimer;
        private float _runPhase;

        private Vector3 _ponytailVelocity;

        /// <summary>Assign cosmetics at runtime (character select).</summary>
        public void SetAppearance(CharacterDefinition def)
        {
            appearance = def;
            ApplyColors();
        }

        private void Awake()
        {
            if (buildOnAwake && transform.Find("Rig") == null)
            {
                Build();
            }
            CacheRig();
            ApplyColors();
        }

        // ------------------------------------------------------------------
        // Rig construction
        // ------------------------------------------------------------------

        private void Build()
        {
            _root = new GameObject("Rig").transform;
            _root.SetParent(transform, false);

            Color primary = appearance != null ? appearance.primaryColor : new Color(0.18f, 0.32f, 0.55f);
            Color skin = appearance != null ? appearance.skinTone : new Color(0.78f, 0.58f, 0.45f);

            _hips = CreatePart("Hips", _root, new Vector3(0f, 0.95f, 0f), new Vector3(0.36f, 0.22f, 0.22f), primary, PrimitiveType.Cube);
            _torso = CreatePart("Torso", _hips, new Vector3(0f, 0.32f, 0f), new Vector3(0.42f, 0.55f, 0.26f), primary, PrimitiveType.Cube);
            _head = CreatePart("Head", _torso, new Vector3(0f, 0.52f, 0.02f), new Vector3(0.22f, 0.26f, 0.24f), skin, PrimitiveType.Cube);

            bool ponytail = appearance == null || appearance.ponytail;
            _ponytail = CreatePart("Ponytail", _head, new Vector3(0f, 0.16f, -0.16f),
                new Vector3(0.08f, 0.22f, 0.08f), new Color(0.16f, 0.12f, 0.1f), PrimitiveType.Cube).transform;
            _ponytail.gameObject.SetActive(ponytail);

            // Arms (mirrored).
            _upperArmL = CreatePart("UpperArmL", _torso, new Vector3(-0.3f, 0.36f, 0f), new Vector3(0.12f, 0.3f, 0.12f), primary, PrimitiveType.Cube);
            _lowerArmL = CreatePart("LowerArmL", _upperArmL, new Vector3(0f, -0.32f, 0f), new Vector3(0.1f, 0.28f, 0.1f), skin, PrimitiveType.Cube);
            _handL = CreatePart("HandL", _lowerArmL, new Vector3(0f, -0.28f, 0f), new Vector3(0.09f, 0.1f, 0.09f), skin, PrimitiveType.Cube);

            _upperArmR = CreatePart("UpperArmR", _torso, new Vector3(0.3f, 0.36f, 0f), new Vector3(0.12f, 0.3f, 0.12f), primary, PrimitiveType.Cube);
            _lowerArmR = CreatePart("LowerArmR", _upperArmR, new Vector3(0f, -0.32f, 0f), new Vector3(0.1f, 0.28f, 0.1f), skin, PrimitiveType.Cube);
            _handR = CreatePart("HandR", _lowerArmR, new Vector3(0f, -0.28f, 0f), new Vector3(0.09f, 0.1f, 0.09f), skin, PrimitiveType.Cube);

            // Legs (mirrored).
            _upperLegL = CreatePart("UpperLegL", _hips, new Vector3(-0.13f, -0.22f, 0f), new Vector3(0.15f, 0.4f, 0.16f), primary, PrimitiveType.Cube);
            _lowerLegL = CreatePart("LowerLegL", _upperLegL, new Vector3(0f, -0.42f, 0f), new Vector3(0.13f, 0.38f, 0.14f), primary, PrimitiveType.Cube);
            _footL = CreatePart("FootL", _lowerLegL, new Vector3(0f, -0.38f, 0.06f), new Vector3(0.13f, 0.08f, 0.26f), new Color(0.12f, 0.12f, 0.14f), PrimitiveType.Cube);

            _upperLegR = CreatePart("UpperLegR", _hips, new Vector3(0.13f, -0.22f, 0f), new Vector3(0.15f, 0.4f, 0.16f), primary, PrimitiveType.Cube);
            _lowerLegR = CreatePart("LowerLegR", _upperLegR, new Vector3(0f, -0.42f, 0f), new Vector3(0.13f, 0.38f, 0.14f), primary, PrimitiveType.Cube);
            _footR = CreatePart("FootR", _lowerLegR, new Vector3(0f, -0.38f, 0.06f), new Vector3(0.13f, 0.08f, 0.26f), new Color(0.12f, 0.12f, 0.14f), PrimitiveType.Cube);

            transform.localScale = Vector3.one * bodyScale;
        }

        private GameObject CreatePart(string partName, Transform parent, Vector3 localPos, Vector3 localScale, Color color, PrimitiveType type)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            part.name = partName;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPos;
            part.transform.localScale = localScale;

            // Strip the default collider - collision is handled by the player's capsule.
            Collider col = part.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }

            Renderer renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Simple Lit")
                    ?? Shader.Find("Universal Render Pipeline/Lit")
                    ?? Shader.Find("Standard"));
                mat.color = color;
                if (mat.HasProperty("_Smoothness"))
                {
                    mat.SetFloat("_Smoothness", 0.35f);
                }
                renderer.sharedMaterial = mat;
            }

            return part;
        }

        private void CacheRig()
        {
            if (_root == null)
            {
                Transform rig = transform.Find("Rig");
                if (rig == null)
                {
                    return;
                }
                _root = rig;
            }

            _hips = _root.Find("Hips");
            _torso = _hips != null ? _hips.Find("Torso") : null;
            _head = _torso != null ? _torso.Find("Head") : null;
            _ponytail = _head != null ? _head.Find("Ponytail") : null;

            if (_torso != null)
            {
                _upperArmL = _torso.Find("UpperArmL");
                _upperArmR = _torso.Find("UpperArmR");
            }
            _lowerArmL = _upperArmL != null ? _upperArmL.Find("LowerArmL") : null;
            _handL = _lowerArmL != null ? _lowerArmL.Find("HandL") : null;
            _lowerArmR = _upperArmR != null ? _upperArmR.Find("LowerArmR") : null;
            _handR = _lowerArmR != null ? _lowerArmR.Find("HandR") : null;

            if (_hips != null)
            {
                _upperLegL = _hips.Find("UpperLegL");
                _upperLegR = _hips.Find("UpperLegR");
            }
            _lowerLegL = _upperLegL != null ? _upperLegL.Find("LowerLegL") : null;
            _footL = _lowerLegL != null ? _lowerLegL.Find("FootL") : null;
            _lowerLegR = _upperLegR != null ? _upperLegR.Find("LowerLegR") : null;
            _footR = _lowerLegR != null ? _lowerLegR.Find("FootR") : null;

            _ragdollBodies.Clear();
        }

        private void ApplyColors()
        {
            if (appearance == null)
            {
                return;
            }
            SetPartColor("Hips", appearance.primaryColor);
            SetPartColor("Torso", appearance.primaryColor);
            SetPartColor("Head", appearance.skinTone);
            SetPartColor("UpperArmL", appearance.primaryColor);
            SetPartColor("UpperArmR", appearance.primaryColor);
            SetPartColor("LowerArmL", appearance.skinTone);
            SetPartColor("LowerArmR", appearance.skinTone);
            SetPartColor("HandL", appearance.skinTone);
            SetPartColor("HandR", appearance.skinTone);
            SetPartColor("UpperLegL", appearance.primaryColor);
            SetPartColor("UpperLegR", appearance.primaryColor);
            SetPartColor("LowerLegL", appearance.primaryColor);
            SetPartColor("LowerLegR", appearance.primaryColor);
        }

        private void SetPartColor(string partName, Color color)
        {
            Transform part = _root != null ? _root.Find($"Hips/{(partName.StartsWith("Head") || partName.StartsWith("Torso") ? partName : partName)}") : null;
            if (part == null)
            {
                part = FindDeep(_root, partName);
            }
            if (part != null && part.TryGetComponent(out Renderer renderer) && renderer.sharedMaterial != null)
            {
                renderer.material.color = color;
            }
        }

        private static Transform FindDeep(Transform parent, string name)
        {
            if (parent == null)
            {
                return null;
            }
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == name)
                {
                    return child;
                }
            }
            return null;
        }

        // ------------------------------------------------------------------
        // Animation state (fed by PlayerController)
        // ------------------------------------------------------------------

        public void SetLocomotion(float speed01, float lean)
        {
            _locomotionSpeed01 = speed01;
            _lean = lean;
        }

        public void SetAirborne(bool value) => _airborne = value;
        public void SetSliding(bool value) => _sliding = value;
        public void SetJetpack(bool value) => _jetpack = value;

        public void TriggerJump() => _jumpPoseTimer = 0.7f;
        public void TriggerSlide() { }
        public void TriggerStumble() => _stumblePoseTimer = 0.6f;

        public void ResetPose()
        {
            _ragdoll = false;
            _jumpPoseTimer = 0f;
            _stumblePoseTimer = 0f;
            _runPhase = 0f;
            if (_root != null)
            {
                foreach (Transform part in _root.GetComponentsInChildren<Transform>(true))
                {
                    if (part.TryGetComponent(out Rigidbody rb))
                    {
                        Destroy(rb);
                    }
                }
            }
            _ragdollBodies.Clear();
            if (_hips != null)
            {
                _hips.localRotation = Quaternion.identity;
            }
        }

        /// <summary>Turns the mannequin into a physical ragdoll on crash.</summary>
        public void EnableRagdoll()
        {
            if (_ragdoll || _root == null)
            {
                return;
            }
            _ragdoll = true;

            foreach (Transform part in _root.GetComponentsInChildren<Transform>(true))
            {
                if (part == _root)
                {
                    continue;
                }
                if (part.TryGetComponent(out Rigidbody rb))
                {
                    _ragdollBodies.Add(rb);
                }
                else
                {
                    rb = part.gameObject.AddComponent<Rigidbody>();
                    rb.mass = 2f;
                    _ragdollBodies.Add(rb);
                }
                if (part.GetComponent<Collider>() == null)
                {
                    BoxCollider box = part.gameObject.AddComponent<BoxCollider>();
                    box.size = Vector3.one * 0.9f;
                }
            }

            // Impact impulse from the crash direction (backwards + up).
            foreach (var rb in _ragdollBodies)
            {
                rb.AddForce(new Vector3(Random.Range(-2f, 2f), Random.Range(3f, 7f), Random.Range(-8f, -3f)), ForceMode.Impulse);
            }
        }

        // ------------------------------------------------------------------
        // Code-driven animation
        // ------------------------------------------------------------------

        private void Update()
        {
            if (_ragdoll || _hips == null)
            {
                return;
            }

            float dt = Time.deltaTime;
            if (_jumpPoseTimer > 0f) _jumpPoseTimer -= dt;
            if (_stumblePoseTimer > 0f) _stumblePoseTimer -= dt;

            AnimateLocomotion(dt);
            AnimatePoseOffsets();
            AnimatePonytail(dt);
        }

        private void AnimateLocomotion(float dt)
        {
            // Run cycle frequency scales with forward speed.
            float frequency = Mathf.Lerp(6.2f, 10.5f, _locomotionSpeed01);
            _runPhase += dt * frequency * Mathf.PI;

            float swing = Mathf.Sin(_runPhase);
            float swingOpp = Mathf.Sin(_runPhase + Mathf.PI);
            float amplitude = Mathf.Lerp(28f, 46f, _locomotionSpeed01);

            if (_airborne || _jetpack)
            {
                // Airborne tuck: legs forward-bent, arms raised.
                SetLocalEuler(_upperLegL, Mathf.Lerp(_upperLegL.localEulerAngles.x, 28f, dt * 8f), 0f, 0f);
                SetLocalEuler(_upperLegR, Mathf.Lerp(_upperLegR.localEulerAngles.x, 12f, dt * 8f), 0f, 0f);
                SetLocalEuler(_lowerLegL, 0f, 0f, Mathf.Lerp(_lowerLegL.localEulerAngles.z, -55f, dt * 8f));
                SetLocalEuler(_lowerLegR, 0f, 0f, Mathf.Lerp(_lowerLegR.localEulerAngles.z, -30f, dt * 8f));
                SetLocalEuler(_upperArmL, -60f, 0f, 0f);
                SetLocalEuler(_upperArmR, -60f, 0f, 0f);
                SetLocalEuler(_lowerArmL, 0f, 0f, -40f);
                SetLocalEuler(_lowerArmR, 0f, 0f, 40f);
            }
            else if (_sliding)
            {
                // Slide: body pitched back, legs out front.
                _hips.localRotation = Quaternion.Slerp(_hips.localRotation, Quaternion.Euler(-72f, 0f, 0f), dt * 10f);
                SetLocalEuler(_upperLegL, 20f, 0f, 0f);
                SetLocalEuler(_upperLegR, 30f, 0f, 0f);
                SetLocalEuler(_lowerLegL, 0f, 0f, -12f);
                SetLocalEuler(_lowerLegR, 0f, 0f, -20f);
                SetLocalEuler(_upperArmL, -95f, 0f, -15f);
                SetLocalEuler(_upperArmR, -95f, 0f, 15f);
            }
            else
            {
                // Standing/running pose.
                float hipPitch = Mathf.Lerp(_hips.localEulerAngles.x, 6f + Mathf.Abs(swing) * 3f, dt * 8f);
                _hips.localRotation = Quaternion.Euler(hipPitch, 0f, 0f);

                SetLocalEuler(_upperLegL, swing * amplitude, 0f, 0f);
                SetLocalEuler(_upperLegR, swingOpp * amplitude, 0f, 0f);

                // Knees bend on the back-swing.
                float kneeL = -Mathf.Max(0f, -swing) * 55f;
                float kneeR = -Mathf.Max(0f, -swingOpp) * 55f;
                SetLocalEuler(_lowerLegL, 0f, 0f, kneeL);
                SetLocalEuler(_lowerLegR, 0f, 0f, kneeR);

                // Arms counter-swing.
                SetLocalEuler(_upperArmL, swingOpp * amplitude * 0.75f, 0f, -6f);
                SetLocalEuler(_upperArmR, swing * amplitude * 0.75f, 0f, 6f);
                SetLocalEuler(_lowerArmL, 0f, 0f, -22f);
                SetLocalEuler(_lowerArmR, 0f, 0f, 22f);

                // Subtle vertical bob + breathing.
                float bob = Mathf.Sin(_runPhase * 2f) * 0.02f * (0.5f + _locomotionSpeed01);
                float breathe = Mathf.Sin(Time.time * 1.6f) * 0.008f;
                if (_torso != null)
                {
                    _torso.localPosition = new Vector3(0f, 0.32f + bob + breathe, 0f);
                    _torso.localRotation = Quaternion.Euler(4f, 0f, 0f);
                }
            }
        }

        private void AnimatePoseOffsets()
        {
            // Body lean into lane changes (z-rotation) + stumble wobble.
            float targetRoll = -_lean * 16f;
            float wobble = _stumblePoseTimer > 0f ? Mathf.Sin(Time.time * 22f) * 9f * _stumblePoseTimer : 0f;

            if (_hips != null && !_sliding)
            {
                Quaternion target = Quaternion.Euler(_hips.localEulerAngles.x, 0f, targetRoll + wobble);
                _hips.localRotation = Quaternion.Slerp(_hips.localRotation, target, Time.deltaTime * 9f);
            }
        }

        private void AnimatePonytail(float dt)
        {
            if (_ponytail == null || !_ponytail.gameObject.activeSelf)
            {
                return;
            }

            // Simple springy pendulum driven by player motion.
            Vector3 worldVel = (transform.position - _lastPosition) / Mathf.Max(dt, 0.0001f);
            _lastPosition = transform.position;

            Vector3 targetEuler = new Vector3(
                Mathf.Clamp(-worldVel.z * 2.2f, -35f, 35f),
                Mathf.Clamp(worldVel.x * 3f, -25f, 25f),
                0f);

            Quaternion target = Quaternion.Euler(targetEuler);
            _ponytail.localRotation = Quaternion.Slerp(_ponytail.localRotation, target, dt * 6f);
        }

        private Vector3 _lastPosition;

        private static void SetLocalEuler(Transform target, float x, float y, float z)
        {
            if (target == null)
            {
                return;
            }
            if (Mathf.Abs(x) > 0.01f || x == 0f)
            {
                // Interpolate toward the target angle for smooth blending.
                Vector3 current = target.localEulerAngles;
                float newX = Mathf.LerpAngle(current.x, x, Time.deltaTime * 12f);
                float newZ = Mathf.LerpAngle(current.z, z, Time.deltaTime * 12f);
                target.localRotation = Quaternion.Euler(newX, y, newZ);
            }
        }
    }
}
