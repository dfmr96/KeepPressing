using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    public class CodeDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text codeText;
        [SerializeField] private ProtocolDirector director;

        private List<int> digits = new List<int>(4);
        private const int MAX_DIGITS = 4;

        private void Start()
        {
            RefreshDisplay();
        }

        public void AddDigit(int digit)
        {
            if (digits.Count >= MAX_DIGITS)
                return;

            digits.Add(digit);
            RefreshDisplay();

            if (digits.Count == MAX_DIGITS)
                CheckCode();
        }

        public void Backspace()
        {
            if (digits.Count == 0)
                return;

            digits.RemoveAt(digits.Count - 1);
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            codeText.text = GetCodeFormatted();
        }

        private string GetCodeFormatted()
        {
            string result = "";
            for (int i = 0; i < MAX_DIGITS; i++)
            {
                result += (i < digits.Count) ? digits[i].ToString() : "_";
                if (i < MAX_DIGITS - 1) result += " ";
            }
            return result;
        }

        private void CheckCode()
        {
            string entered = string.Join("", digits);

            string usedCodeName;
            int codeIndex = director.ValidateCode(entered, out usedCodeName);

            if (codeIndex >= 0)
            {
                Debug.Log($"✔ Código CORRECTO - {usedCodeName} utilizado");
                director.OnCorrectCodeEntered(codeIndex, usedCodeName);
            }
            else
            {
                Debug.Log("✘ Código INCORRECTO");
                director.OnIncorrectCodeEntered();
            }

            digits.Clear();
            RefreshDisplay();
        }
    }
}