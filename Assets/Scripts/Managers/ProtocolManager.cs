using UnityEngine;
using UnityEngine.Events;
using KeepPressing.Core;

namespace KeepPressing.Managers
{
    /// <summary>
    /// Manages the overall game state and protocol cycles.
    /// Tracks successful/failed inputs, handles timer interval switching,
    /// and triggers the final "system failure" state.
    /// </summary>
    public class ProtocolManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CodeInputPanel codeInputPanel;
        [SerializeField] private CountdownTimer countdownTimer;

        [Header("Protocol Settings")]
        [SerializeField] private float initialTimerDuration = 120f;
        [SerializeField] private float finalActDuration = 60f;
        [SerializeField] private int cyclesBeforeFinalAct = 3;
        [SerializeField] private bool enableFinalActSwitch = true;

        [Header("Game State")]
        [SerializeField] private int currentCycleCount = 0;
        [SerializeField] private int successfulCycles = 0;
        [SerializeField] private int failedCycles = 0;
        [SerializeField] private bool isInFinalAct = false;
        [SerializeField] private bool protocolSuspended = false;

        [Header("Events")]
        public UnityEvent OnProtocolStarted;
        public UnityEvent OnCycleCompleted;
        public UnityEvent OnCycleFailed;
        public UnityEvent OnFinalActTriggered;
        public UnityEvent OnProtocolSuspended;
        public UnityEvent OnHatchUnlocked;

        public int CurrentCycleCount => currentCycleCount;
        public int SuccessfulCycles => successfulCycles;
        public int FailedCycles => failedCycles;
        public bool IsInFinalAct => isInFinalAct;
        public bool ProtocolSuspended => protocolSuspended;

        private static ProtocolManager instance;
        public static ProtocolManager Instance => instance;

        private void Awake()
        {
            // Singleton pattern
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void Start()
        {
            SetupListeners();
            StartProtocol();
        }

        private void SetupListeners()
        {
            if (codeInputPanel != null)
            {
                codeInputPanel.OnCodeCorrect.AddListener(OnCodeCorrectEntered);
                codeInputPanel.OnCodeIncorrect.AddListener(OnCodeIncorrectEntered);
            }

            if (countdownTimer != null)
            {
                countdownTimer.OnTimerExpired.AddListener(OnTimerExpired);
            }
        }

        /// <summary>
        /// Starts the protocol system
        /// </summary>
        public void StartProtocol()
        {
            if (protocolSuspended)
            {
                Debug.Log("Protocol is suspended. Cannot start.");
                return;
            }

            Debug.Log("MANTIS Protocol initiated. First cycle starting...");

            currentCycleCount = 0;
            successfulCycles = 0;
            failedCycles = 0;
            isInFinalAct = false;

            if (countdownTimer != null)
            {
                countdownTimer.SetDuration(initialTimerDuration);
                countdownTimer.RestartTimer();
            }

            OnProtocolStarted?.Invoke();
        }

        /// <summary>
        /// Called when a correct code is entered
        /// </summary>
        private void OnCodeCorrectEntered()
        {
            if (protocolSuspended) return;

            successfulCycles++;
            currentCycleCount++;

            Debug.Log($"Cycle {currentCycleCount} completed successfully. Total successful: {successfulCycles}");

            OnCycleCompleted?.Invoke();

            // Check if we should trigger final act
            if (enableFinalActSwitch && !isInFinalAct && successfulCycles >= cyclesBeforeFinalAct)
            {
                TriggerFinalAct();
            }
        }

        /// <summary>
        /// Called when an incorrect code is entered
        /// </summary>
        private void OnCodeIncorrectEntered()
        {
            if (protocolSuspended) return;

            Debug.Log("Incorrect code entered. Code cleared, timer continues.");
            // Timer continues, no cycle count increase
        }

        /// <summary>
        /// Called when timer expires without successful code entry
        /// </summary>
        private void OnTimerExpired()
        {
            if (protocolSuspended) return;

            failedCycles++;
            currentCycleCount++;

            Debug.LogWarning($"Cycle {currentCycleCount} FAILED. Timer expired. Total failures: {failedCycles}");

            OnCycleFailed?.Invoke();

            // In final act, first failure suspends protocol
            if (isInFinalAct && failedCycles >= 1)
            {
                SuspendProtocol();
            }
            else
            {
                // Continue with next cycle
                if (countdownTimer != null)
                {
                    countdownTimer.RestartTimer();
                }
            }
        }

        /// <summary>
        /// Triggers the final act with accelerated timer
        /// </summary>
        private void TriggerFinalAct()
        {
            isInFinalAct = true;

            Debug.Log($"SYSTEM ALERT: Switching to accelerated protocol. Timer now: {finalActDuration}s");

            if (countdownTimer != null)
            {
                countdownTimer.SetDuration(finalActDuration);
                countdownTimer.RestartTimer();
            }

            OnFinalActTriggered?.Invoke();
        }

        /// <summary>
        /// Suspends the protocol and unlocks the hatch
        /// </summary>
        private void SuspendProtocol()
        {
            protocolSuspended = true;

            Debug.Log("PROTOCOL SUSPENDED. System failure detected. Unsealing emergency hatch...");

            if (countdownTimer != null)
            {
                countdownTimer.PauseTimer();
            }

            if (codeInputPanel != null)
            {
                codeInputPanel.LockPanel();
            }

            OnProtocolSuspended?.Invoke();

            // Trigger hatch unlock after a dramatic pause
            Invoke(nameof(UnlockHatch), 2f);
        }

        /// <summary>
        /// Unlocks the hidden hatch
        /// </summary>
        private void UnlockHatch()
        {
            Debug.Log("Emergency hatch UNLOCKED. Protocol override complete.");
            OnHatchUnlocked?.Invoke();
        }

        /// <summary>
        /// Resets the protocol to initial state (for debugging/testing)
        /// </summary>
        public void ResetProtocol()
        {
            currentCycleCount = 0;
            successfulCycles = 0;
            failedCycles = 0;
            isInFinalAct = false;
            protocolSuspended = false;

            if (codeInputPanel != null)
            {
                codeInputPanel.UnlockPanel();
                codeInputPanel.ClearCode();
            }

            if (countdownTimer != null)
            {
                countdownTimer.SetDuration(initialTimerDuration);
                countdownTimer.ResetTimer();
            }

            Debug.Log("Protocol reset to initial state.");
        }

        /// <summary>
        /// Manually triggers the final act (for testing)
        /// </summary>
        public void ForceTriggerFinalAct()
        {
            TriggerFinalAct();
        }

        /// <summary>
        /// Gets current protocol status as a string
        /// </summary>
        public string GetProtocolStatus()
        {
            if (protocolSuspended)
            {
                return "PROTOCOL SUSPENDED";
            }

            string status = $"Cycle: {currentCycleCount} | Success: {successfulCycles} | Failed: {failedCycles}";
            if (isInFinalAct)
            {
                status += " | FINAL ACT";
            }
            return status;
        }

        private void OnDestroy()
        {
            if (codeInputPanel != null)
            {
                codeInputPanel.OnCodeCorrect.RemoveListener(OnCodeCorrectEntered);
                codeInputPanel.OnCodeIncorrect.RemoveListener(OnCodeIncorrectEntered);
            }

            if (countdownTimer != null)
            {
                countdownTimer.OnTimerExpired.RemoveListener(OnTimerExpired);
            }

            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
