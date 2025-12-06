using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Core
{
    /// <summary>
    /// When valve is placed in socket, disables grab and automatically rotates 180 degrees
    /// </summary>
    public class SocketValveBehavior : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private XRSocketInteractor socketInteractor;

        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem particleSystem1;
        [SerializeField] private ParticleSystem particleSystem2;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationDuration = 2f;
        [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Events")]
        public UnityEvent OnRotationComplete;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = false;

        private bool isRotating = false;
        private float rotationProgress = 0f;
        private Transform valveTransform = null;
        private XRGrabInteractable valveGrabInteractable = null;
        private float initialRateOverTime1 = 0f;
        private float initialRateOverTime2 = 0f;

        private void Start()
        {
            if (socketInteractor == null)
            {
                socketInteractor = GetComponent<XRSocketInteractor>();
            }

            if (socketInteractor == null)
            {
                Debug.LogError("[SocketValveBehavior] XRSocketInteractor component not found!");
                enabled = false;
                return;
            }

            // Subscribe to socket events
            socketInteractor.selectEntered.AddListener(OnValveSocketed);

            if (showDebugLogs)
            {
                Debug.Log($"[SocketValveBehavior] Initialized on {gameObject.name}");
            }
        }

        private void OnDestroy()
        {
            if (socketInteractor != null)
            {
                socketInteractor.selectEntered.RemoveListener(OnValveSocketed);
            }
        }

        private void OnValveSocketed(SelectEnterEventArgs args)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[SocketValveBehavior] Valve socketed: {args.interactableObject.transform.name}");
            }

            valveTransform = args.interactableObject.transform;
            valveGrabInteractable = valveTransform.GetComponent<XRGrabInteractable>();

            // Disable grab immediately to prevent player from removing it
            if (valveGrabInteractable != null)
            {
                valveGrabInteractable.enabled = false;

                if (showDebugLogs)
                    Debug.Log("[SocketValveBehavior] XRGrabInteractable disabled - valve locked in socket");
            }

            // Start automatic rotation
            isRotating = true;
            rotationProgress = 0f;

            // Store initial particle emission rates
            if (particleSystem1 != null)
            {
                var emission1 = particleSystem1.emission;
                initialRateOverTime1 = emission1.rateOverTime.constant;

                if (showDebugLogs)
                    Debug.Log($"[SocketValveBehavior] Particle1 initial rate: {initialRateOverTime1}");
            }

            if (particleSystem2 != null)
            {
                var emission2 = particleSystem2.emission;
                initialRateOverTime2 = emission2.rateOverTime.constant;

                if (showDebugLogs)
                    Debug.Log($"[SocketValveBehavior] Particle2 initial rate: {initialRateOverTime2}");
            }

            if (showDebugLogs)
                Debug.Log("[SocketValveBehavior] Starting automatic 180° rotation");
        }

        private void Update()
        {
            if (!isRotating || valveTransform == null)
                return;

            // Increment rotation progress
            rotationProgress += Time.deltaTime / rotationDuration;

            if (rotationProgress >= 1f)
            {
                // Rotation complete
                rotationProgress = 1f;
                isRotating = false;

                // Snap to final rotation
                valveTransform.localRotation = Quaternion.Euler(0, 90, 180f);

                // Set particle emission to zero
                SetParticleEmissionToZero();

                if (showDebugLogs)
                    Debug.Log("[SocketValveBehavior] Rotation complete at 180°");

                // Trigger event
                OnRotationComplete?.Invoke();
            }
            else
            {
                // Interpolate rotation using curve
                float curveValue = rotationCurve.Evaluate(rotationProgress);
                float currentAngle = Mathf.Lerp(0f, 180f, curveValue);

                valveTransform.localRotation = Quaternion.Euler(0, 90, currentAngle);

                // Update particle emission rates (decrease from initial to 0)
                UpdateParticleEmission(curveValue);
            }
        }

        private void UpdateParticleEmission(float progress)
        {
            // Progress goes from 0 to 1, we want emission to go from initial to 0
            if (particleSystem1 != null)
            {
                var emission1 = particleSystem1.emission;
                float newRate1 = Mathf.Lerp(initialRateOverTime1, 0f, progress);
                emission1.rateOverTime = newRate1;
            }

            if (particleSystem2 != null)
            {
                var emission2 = particleSystem2.emission;
                float newRate2 = Mathf.Lerp(initialRateOverTime2, 0f, progress);
                emission2.rateOverTime = newRate2;
            }
        }

        private void SetParticleEmissionToZero()
        {
            if (particleSystem1 != null)
            {
                var emission1 = particleSystem1.emission;
                emission1.rateOverTime = 0f;

                if (showDebugLogs)
                    Debug.Log("[SocketValveBehavior] Particle1 emission set to 0");
            }

            if (particleSystem2 != null)
            {
                var emission2 = particleSystem2.emission;
                emission2.rateOverTime = 0f;

                if (showDebugLogs)
                    Debug.Log("[SocketValveBehavior] Particle2 emission set to 0");
            }
        }
    }
}
