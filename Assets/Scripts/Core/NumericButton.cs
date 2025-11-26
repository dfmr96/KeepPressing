using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace KeepPressing.Core
{
    /// <summary>
    /// Interactive numeric button for the code input panel.
    /// Uses XR Interaction Toolkit for VR poke/press interactions.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class NumericButton : MonoBehaviour
    {
        [Header("Button Settings")]
        [SerializeField] private int buttonValue;
        [SerializeField] private bool isSpecialButton = false; // For Clear/Submit buttons

        [Header("Visual Feedback")]
        [SerializeField] private Transform buttonVisual;
        [SerializeField] private float pressDepth = 0.01f;
        [SerializeField] private float pressSpeed = 10f;
        [SerializeField] private MeshRenderer buttonRenderer;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material pressedMaterial;

        [Header("Audio Feedback")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pressSound;
        [SerializeField] private AudioClip releaseSound;

        [Header("Haptic Feedback")]
        [SerializeField] private float hapticIntensity = 0.5f;
        [SerializeField] private float hapticDuration = 0.1f;

        [Header("Events")]
        public UnityEvent<int> OnButtonPressed;

        private XRSimpleInteractable interactable;
        private Vector3 defaultPosition;
        private Vector3 pressedPosition;
        private bool isPressed = false;
        private float currentDepth = 0f;

        public int ButtonValue => buttonValue;
        public bool IsSpecialButton => isSpecialButton;

        private void Awake()
        {
            interactable = GetComponent<XRSimpleInteractable>();

            if (buttonVisual != null)
            {
                defaultPosition = buttonVisual.localPosition;
                pressedPosition = defaultPosition - new Vector3(0, pressDepth, 0);
            }

            SetupInteractable();
        }

        private void Start()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                    audioSource.spatialBlend = 1f; // 3D sound
                }
            }
        }

        private void Update()
        {
            if (buttonVisual != null)
            {
                float targetDepth = isPressed ? 1f : 0f;
                currentDepth = Mathf.Lerp(currentDepth, targetDepth, Time.deltaTime * pressSpeed);
                buttonVisual.localPosition = Vector3.Lerp(defaultPosition, pressedPosition, currentDepth);
            }
        }

        private void SetupInteractable()
        {
            if (interactable == null) return;

            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.selectExited.AddListener(OnSelectExited);
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            PressButton();
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            ReleaseButton();
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            // Could add hover visual feedback here
        }

        private void OnHoverExited(HoverExitEventArgs args)
        {
            // Could remove hover visual feedback here
        }

        /// <summary>
        /// Triggers button press with all feedback
        /// </summary>
        public void PressButton()
        {
            if (isPressed) return;

            isPressed = true;

            // Visual feedback
            if (buttonRenderer != null && pressedMaterial != null)
            {
                buttonRenderer.material = pressedMaterial;
            }

            // Audio feedback
            if (audioSource != null && pressSound != null)
            {
                audioSource.PlayOneShot(pressSound);
            }

            // Haptic feedback
            TriggerHapticFeedback();

            // Invoke event
            OnButtonPressed?.Invoke(buttonValue);
        }

        /// <summary>
        /// Releases button and returns to default state
        /// </summary>
        public void ReleaseButton()
        {
            if (!isPressed) return;

            isPressed = false;

            // Visual feedback
            if (buttonRenderer != null && defaultMaterial != null)
            {
                buttonRenderer.material = defaultMaterial;
            }

            // Audio feedback
            if (audioSource != null && releaseSound != null)
            {
                audioSource.PlayOneShot(releaseSound);
            }
        }

        /// <summary>
        /// Triggers haptic feedback on the interacting controller
        /// </summary>
        private void TriggerHapticFeedback()
        {
            if (interactable == null || !interactable.isSelected) return;

            var interactors = interactable.interactorsSelecting;
            if (interactors.Count == 0) return;

            foreach (var interactor in interactors)
            {
                if (interactor is XRBaseControllerInteractor controllerInteractor)
                {
                    controllerInteractor.SendHapticImpulse(hapticIntensity, hapticDuration);
                }
            }
        }

        /// <summary>
        /// Sets the button value (useful for dynamic button creation)
        /// </summary>
        public void SetButtonValue(int value)
        {
            buttonValue = value;
        }

        /// <summary>
        /// Enables or disables the button
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            if (interactable != null)
            {
                interactable.enabled = enabled;
            }
        }

        private void OnDestroy()
        {
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(OnSelectEntered);
                interactable.selectExited.RemoveListener(OnSelectExited);
                interactable.hoverEntered.RemoveListener(OnHoverEntered);
                interactable.hoverExited.RemoveListener(OnHoverExited);
            }
        }
    }
}
