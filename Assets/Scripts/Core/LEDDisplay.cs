using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace KeepPressing.Core
{
    /// <summary>
    /// Digital LED display for showing the entered code sequence.
    /// Displays numbers with underscore placeholders for empty slots.
    /// </summary>
    public class LEDDisplay : MonoBehaviour
    {
        [Header("Display Settings")]
        [SerializeField] private TextMeshProUGUI displayText;
        [SerializeField] private int maxDigits = 6;
        [SerializeField] private char placeholderChar = '_';
        [SerializeField] private float digitSpacing = 0.5f;

        [Header("Visual Feedback")]
        [SerializeField] private Color normalColor = new Color(0f, 1f, 0f, 1f); // Green
        [SerializeField] private Color errorColor = new Color(1f, 0f, 0f, 1f); // Red
        [SerializeField] private Color successColor = new Color(0f, 1f, 1f, 1f); // Cyan
        [SerializeField] private float errorFlashDuration = 0.5f;

        private List<int> currentDigits = new List<int>();
        private bool isFlashing = false;
        private float flashTimer = 0f;

        private void Awake()
        {
            if (displayText == null)
            {
                displayText = GetComponent<TextMeshProUGUI>();
            }

            if (displayText == null)
            {
                Debug.LogError("LEDDisplay: No TextMeshProUGUI component found!");
            }
        }

        private void Start()
        {
            Clear();
        }

        private void Update()
        {
            if (isFlashing)
            {
                flashTimer -= Time.deltaTime;
                if (flashTimer <= 0)
                {
                    isFlashing = false;
                    UpdateDisplay();
                }
            }
        }

        /// <summary>
        /// Adds a digit to the display
        /// </summary>
        public bool AddDigit(int digit)
        {
            if (currentDigits.Count >= maxDigits)
            {
                return false;
            }

            if (digit < 0 || digit > 99)
            {
                Debug.LogWarning($"LEDDisplay: Invalid digit {digit}. Must be between 0 and 99.");
                return false;
            }

            currentDigits.Add(digit);
            UpdateDisplay();
            return true;
        }

        /// <summary>
        /// Removes the last digit from the display
        /// </summary>
        public bool RemoveLastDigit()
        {
            if (currentDigits.Count == 0)
            {
                return false;
            }

            currentDigits.RemoveAt(currentDigits.Count - 1);
            UpdateDisplay();
            return true;
        }

        /// <summary>
        /// Clears all digits from the display
        /// </summary>
        public void Clear()
        {
            currentDigits.Clear();
            UpdateDisplay();
        }

        /// <summary>
        /// Gets the current sequence of entered digits
        /// </summary>
        public List<int> GetCurrentSequence()
        {
            return new List<int>(currentDigits);
        }

        /// <summary>
        /// Returns true if the display is full
        /// </summary>
        public bool IsFull()
        {
            return currentDigits.Count >= maxDigits;
        }

        /// <summary>
        /// Returns the number of digits currently displayed
        /// </summary>
        public int GetDigitCount()
        {
            return currentDigits.Count;
        }

        /// <summary>
        /// Flashes the display red to indicate an error
        /// </summary>
        public void ShowError()
        {
            if (displayText != null)
            {
                displayText.color = errorColor;
                isFlashing = true;
                flashTimer = errorFlashDuration;
            }
        }

        /// <summary>
        /// Shows success state (cyan color)
        /// </summary>
        public void ShowSuccess()
        {
            if (displayText != null)
            {
                displayText.color = successColor;
            }
        }

        /// <summary>
        /// Updates the display text based on current digits
        /// </summary>
        private void UpdateDisplay()
        {
            if (displayText == null) return;

            if (!isFlashing)
            {
                displayText.color = normalColor;
            }

            string displayString = "";

            for (int i = 0; i < currentDigits.Count; i++)
            {
                displayString += currentDigits[i].ToString("D2");
                if (i < currentDigits.Count - 1 || currentDigits.Count < maxDigits)
                {
                    displayString += " ";
                }
            }

            for (int i = currentDigits.Count; i < maxDigits; i++)
            {
                displayString += placeholderChar.ToString() + placeholderChar.ToString();
                if (i < maxDigits - 1)
                {
                    displayString += " ";
                }
            }

            displayText.text = displayString;
        }

        /// <summary>
        /// Sets the display to show a specific sequence (for debugging/testing)
        /// </summary>
        public void SetSequence(List<int> sequence)
        {
            currentDigits.Clear();
            foreach (int digit in sequence)
            {
                if (currentDigits.Count < maxDigits)
                {
                    currentDigits.Add(digit);
                }
            }
            UpdateDisplay();
        }
    }
}
