using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace KeepPressing.Core
{
    /// <summary>
    /// Main controller for the code input panel.
    /// Manages code validation, input logic, and integration with timer and display.
    /// The correct code is the LOST bunker sequence: 4 8 15 16 23 42
    /// </summary>
    public class CodeInputPanel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private LEDDisplay ledDisplay;
        [SerializeField] private CountdownTimer timer;
        [SerializeField] private NumericButton[] numericButtons;

        [Header("Special Buttons")]
        [SerializeField] private NumericButton clearButton;
        [SerializeField] private NumericButton submitButton;

        [Header("Code Settings")]
        [SerializeField] private List<int> correctCode = new List<int> { 4, 8, 15, 16, 23, 42 };
        [SerializeField] private bool autoSubmitOnComplete = true;

        [Header("Audio Feedback")]
        [SerializeField] private AudioSource panelAudioSource;
        [SerializeField] private AudioClip correctCodeSound;
        [SerializeField] private AudioClip incorrectCodeSound;
        [SerializeField] private AudioClip clearSound;

        [Header("Events")]
        public UnityEvent OnCodeCorrect;
        public UnityEvent OnCodeIncorrect;
        public UnityEvent OnCodeCleared;
        public UnityEvent<int> OnDigitEntered;

        private List<int> currentInput = new List<int>();
        private bool isPanelLocked = false;

        private void Start()
        {
            SetupButtons();
            SetupTimer();

            if (ledDisplay == null)
            {
                ledDisplay = GetComponentInChildren<LEDDisplay>();
            }

            if (timer == null)
            {
                timer = GetComponentInChildren<CountdownTimer>();
            }
        }

        private void SetupButtons()
        {
            if (numericButtons != null)
            {
                foreach (var button in numericButtons)
                {
                    if (button != null)
                    {
                        button.OnButtonPressed.AddListener(OnNumericButtonPressed);
                    }
                }
            }

            if (clearButton != null)
            {
                clearButton.OnButtonPressed.AddListener((_) => ClearCode());
            }

            if (submitButton != null)
            {
                submitButton.OnButtonPressed.AddListener((_) => SubmitCode());
            }
        }

        private void SetupTimer()
        {
            if (timer != null)
            {
                timer.OnTimerExpired.AddListener(OnTimerExpired);
            }
        }

        /// <summary>
        /// Handles numeric button presses
        /// </summary>
        private void OnNumericButtonPressed(int value)
        {
            if (isPanelLocked) return;

            if (currentInput.Count >= correctCode.Count)
            {
                // Already at max capacity
                return;
            }

            currentInput.Add(value);
            ledDisplay?.AddDigit(value);
            OnDigitEntered?.Invoke(value);

            // Auto-submit when complete
            if (autoSubmitOnComplete && currentInput.Count == correctCode.Count)
            {
                Invoke(nameof(SubmitCode), 0.3f); // Small delay for player feedback
            }
        }

        /// <summary>
        /// Submits the current code for validation
        /// </summary>
        public void SubmitCode()
        {
            if (isPanelLocked) return;

            if (currentInput.Count != correctCode.Count)
            {
                Debug.Log("Code incomplete. Please enter all digits.");
                return;
            }

            bool isCorrect = ValidateCode();

            if (isCorrect)
            {
                OnCodeCorrectEntered();
            }
            else
            {
                OnCodeIncorrectEntered();
            }
        }

        /// <summary>
        /// Validates the current input against the correct code
        /// </summary>
        private bool ValidateCode()
        {
            if (currentInput.Count != correctCode.Count)
            {
                return false;
            }

            return currentInput.SequenceEqual(correctCode);
        }

        /// <summary>
        /// Called when correct code is entered
        /// </summary>
        private void OnCodeCorrectEntered()
        {
            Debug.Log("CORRECT CODE! Protocol cycle completed.");

            ledDisplay?.ShowSuccess();

            if (panelAudioSource != null && correctCodeSound != null)
            {
                panelAudioSource.PlayOneShot(correctCodeSound);
            }

            OnCodeCorrect?.Invoke();

            // Reset for next cycle
            Invoke(nameof(ResetForNextCycle), 2f);
        }

        /// <summary>
        /// Called when incorrect code is entered
        /// </summary>
        private void OnCodeIncorrectEntered()
        {
            Debug.Log("INCORRECT CODE! Clearing input.");

            ledDisplay?.ShowError();

            if (panelAudioSource != null && incorrectCodeSound != null)
            {
                panelAudioSource.PlayOneShot(incorrectCodeSound);
            }

            OnCodeIncorrect?.Invoke();

            // Clear after showing error
            Invoke(nameof(ClearCode), 1f);
        }

        /// <summary>
        /// Clears the current code input
        /// </summary>
        public void ClearCode()
        {
            currentInput.Clear();
            ledDisplay?.Clear();

            if (panelAudioSource != null && clearSound != null)
            {
                panelAudioSource.PlayOneShot(clearSound);
            }

            OnCodeCleared?.Invoke();
        }

        /// <summary>
        /// Called when timer expires without code entry
        /// </summary>
        private void OnTimerExpired()
        {
            Debug.LogWarning("TIMER EXPIRED! Protocol failure.");
            // The ProtocolManager will handle the consequences
        }

        /// <summary>
        /// Resets panel for the next input cycle
        /// </summary>
        private void ResetForNextCycle()
        {
            ClearCode();
            timer?.RestartTimer();
        }

        /// <summary>
        /// Locks the panel, preventing any input
        /// </summary>
        public void LockPanel()
        {
            isPanelLocked = true;
            SetButtonsEnabled(false);
        }

        /// <summary>
        /// Unlocks the panel, allowing input
        /// </summary>
        public void UnlockPanel()
        {
            isPanelLocked = false;
            SetButtonsEnabled(true);
        }

        /// <summary>
        /// Enables or disables all buttons
        /// </summary>
        private void SetButtonsEnabled(bool enabled)
        {
            if (numericButtons != null)
            {
                foreach (var button in numericButtons)
                {
                    button?.SetEnabled(enabled);
                }
            }

            clearButton?.SetEnabled(enabled);
            submitButton?.SetEnabled(enabled);
        }

        /// <summary>
        /// Gets the current input sequence
        /// </summary>
        public List<int> GetCurrentInput()
        {
            return new List<int>(currentInput);
        }

        /// <summary>
        /// Sets a custom correct code (for testing or different puzzles)
        /// </summary>
        public void SetCorrectCode(List<int> newCode)
        {
            correctCode = new List<int>(newCode);
        }

        /// <summary>
        /// Gets the correct code (for debugging)
        /// </summary>
        public List<int> GetCorrectCode()
        {
            return new List<int>(correctCode);
        }

        private void OnDestroy()
        {
            if (numericButtons != null)
            {
                foreach (var button in numericButtons)
                {
                    if (button != null)
                    {
                        button.OnButtonPressed.RemoveListener(OnNumericButtonPressed);
                    }
                }
            }

            if (timer != null)
            {
                timer.OnTimerExpired.RemoveListener(OnTimerExpired);
            }
        }
    }
}
