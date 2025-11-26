using Core;
using UnityEngine;
using KeepPressing.Core;

namespace KeepPressing.Core
{
    /// <summary>
    /// Debug helper for CodeInputPanel. Shows in console what's happening.
    /// Attach to CodeInputPanel GameObject to diagnose issues.
    /// </summary>
    public class DebugCodePanel : MonoBehaviour
    {
        [Header("Components to Check")]
        [SerializeField] private CodeInputPanel codePanel;
        [SerializeField] private LEDDisplay ledDisplay;
        [SerializeField] private NumericButton[] buttons;

        [Header("Debug Info")]
        [SerializeField] private bool logButtonPresses = true;
        [SerializeField] private bool logDisplayUpdates = true;

        private void Start()
        {
            // Auto-find components
            if (codePanel == null)
                codePanel = GetComponent<CodeInputPanel>();

            if (ledDisplay == null)
                ledDisplay = GetComponentInChildren<LEDDisplay>();

            if (buttons == null || buttons.Length == 0)
                buttons = GetComponentsInChildren<NumericButton>();

            // Verify setup
            Debug.Log("=== CODE PANEL DEBUG START ===");
            Debug.Log($"CodeInputPanel found: {codePanel != null}");
            Debug.Log($"LEDDisplay found: {ledDisplay != null}");
            Debug.Log($"NumericButtons found: {buttons?.Length ?? 0}");

            if (ledDisplay != null)
            {
                var textComponent = ledDisplay.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                Debug.Log($"TextMeshPro in display: {textComponent != null}");
                if (textComponent != null)
                {
                    Debug.Log($"Display text current value: '{textComponent.text}'");
                    Debug.Log($"Display position: {ledDisplay.transform.position}");
                    Debug.Log($"Display active: {ledDisplay.gameObject.activeInHierarchy}");
                }
            }

            // Hook up debug listeners to buttons
            if (buttons != null)
            {
                foreach (var button in buttons)
                {
                    if (button != null)
                    {
                        button.OnButtonPressed.AddListener((value) => OnDebugButtonPressed(button, value));
                        Debug.Log($"Button setup: {button.name}, Value: {button.ButtonValue}, IsSpecial: {button.IsSpecialButton}");
                    }
                }
            }

            // Hook up to panel events
            if (codePanel != null)
            {
                codePanel.OnDigitEntered.AddListener(OnDigitEntered);
                codePanel.OnCodeCorrect.AddListener(() => Debug.Log("✓ CODE CORRECT!"));
                codePanel.OnCodeIncorrect.AddListener(() => Debug.Log("✗ CODE INCORRECT!"));
                codePanel.OnCodeCleared.AddListener(() => Debug.Log("⟲ CODE CLEARED"));
            }

            Debug.Log("=== CODE PANEL DEBUG END ===");
        }

        private void OnDebugButtonPressed(NumericButton button, int value)
        {
            if (logButtonPresses)
            {
                Debug.Log($">>> BUTTON PRESSED: {button.name} | Value: {value} | IsSpecial: {button.IsSpecialButton}");
            }
        }

        private void OnDigitEntered(int digit)
        {
            if (logDisplayUpdates)
            {
                Debug.Log($">>> DIGIT ENTERED: {digit}");
                if (ledDisplay != null)
                {
                    var currentSequence = ledDisplay.GetCurrentSequence();
                    Debug.Log($">>> CURRENT SEQUENCE: [{string.Join(", ", currentSequence)}]");
                }
            }
        }

        [ContextMenu("Test Add Digit Manually")]
        public void TestAddDigit()
        {
            if (ledDisplay != null)
            {
                bool success = ledDisplay.AddDigit(4);
                Debug.Log($"Manual digit add (4): {(success ? "SUCCESS" : "FAILED")}");
                var textComponent = ledDisplay.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (textComponent != null)
                {
                    Debug.Log($"Display now shows: '{textComponent.text}'");
                    Debug.Log($"Text GameObject active: {textComponent.gameObject.activeInHierarchy}");
                    Debug.Log($"Text position (world): {textComponent.transform.position}");
                    Debug.Log($"Text enabled: {textComponent.enabled}");
                    Debug.Log($"Text color: {textComponent.color}");
                    Debug.Log($"Text canvas: {textComponent.canvas?.name ?? "NULL CANVAS"}");
                }
            }
            else
            {
                Debug.LogError("LEDDisplay not found!");
            }
        }

        [ContextMenu("Test Clear Display")]
        public void TestClearDisplay()
        {
            if (ledDisplay != null)
            {
                ledDisplay.Clear();
                Debug.Log("Display cleared");
            }
        }

        [ContextMenu("Check Component Links")]
        public void CheckLinks()
        {
            Debug.Log("=== CHECKING COMPONENT LINKS ===");

            // Check CodeInputPanel serialized fields
            if (codePanel != null)
            {
                Debug.Log("Checking CodeInputPanel internal references...");
                Debug.Log("(Open CodeInputPanel in Inspector to verify these are assigned)");

                // Check if display is working
                if (ledDisplay != null)
                {
                    Debug.Log($"✓ LEDDisplay reference exists");
                    var tmp = ledDisplay.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        Debug.Log($"✓ TextMeshPro found: '{tmp.text}'");
                    }
                    else
                    {
                        Debug.LogError("✗ TextMeshPro NOT FOUND in LEDDisplay!");
                    }
                }
                else
                {
                    Debug.LogError("✗ LEDDisplay reference is NULL!");
                }
            }

            Debug.Log("=== CHECK COMPLETE ===");
        }

        private void Update()
        {
            // Keyboard shortcuts for testing
            if (Input.GetKeyDown(KeyCode.Alpha4))
                SimulateButtonPress(4);
            if (Input.GetKeyDown(KeyCode.Alpha8))
                SimulateButtonPress(8);
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SimulateButtonPress(15);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                SimulateButtonPress(16);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                SimulateButtonPress(23);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                SimulateButtonPress(42);
            if (Input.GetKeyDown(KeyCode.C))
                codePanel?.ClearCode();
            if (Input.GetKeyDown(KeyCode.Return))
                codePanel?.SubmitCode();
        }

        private void SimulateButtonPress(int value)
        {
            Debug.Log($"🎮 SIMULATING BUTTON: {value}");

            // Find button with this value
            var button = System.Array.Find(buttons, b => b != null && b.ButtonValue == value);
            if (button != null)
            {
                // Manually invoke the button pressed event
                button.OnButtonPressed?.Invoke(value);
            }
            else
            {
                Debug.LogWarning($"Button with value {value} not found!");
            }
        }
    }
}
