using UnityEngine;
using KeepPressing.Core;

namespace Core
{
    /// <summary>
    /// Quick setup script for testing the code input panel.
    /// Attach to CodeInputPanel GameObject and click "Create Test Setup" in inspector.
    /// </summary>
    public class QuickTestSetup : MonoBehaviour
    {
        [ContextMenu("Create Test Setup")]
        public void CreateTestSetup()
        {
            // Create Timer
            GameObject timerObj = new GameObject("Timer");
            timerObj.transform.SetParent(transform);
            CountdownTimer timer = timerObj.AddComponent<CountdownTimer>();

            // Create Display
            GameObject displayObj = new GameObject("Display");
            displayObj.transform.SetParent(transform);
            displayObj.transform.localPosition = new Vector3(0, 0.15f, 0);

            // Add Canvas
            Canvas canvas = displayObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            displayObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 200);
            displayObj.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

            // Add TextMeshPro
            GameObject textObj = new GameObject("LEDText");
            textObj.transform.SetParent(displayObj.transform);
            var tmp = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.fontSize = 100;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.color = Color.green;
            textObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 200);
            textObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            LEDDisplay ledDisplay = displayObj.AddComponent<LEDDisplay>();

            // Create button grid
            GameObject buttonGrid = new GameObject("ButtonGrid");
            buttonGrid.transform.SetParent(transform);
            buttonGrid.transform.localPosition = Vector3.zero;

            int[] buttonValues = { 4, 8, 15, 16, 23, 42 };
            NumericButton[] buttons = new NumericButton[6];

            for (int i = 0; i < buttonValues.Length; i++)
            {
                // Calculate position in 2x3 grid
                float x = (i % 3) * 0.1f - 0.1f;
                float y = -(i / 3) * 0.1f;

                GameObject buttonObj = CreateButton(buttonValues[i], new Vector3(x, y, 0));
                buttonObj.transform.SetParent(buttonGrid.transform);
                buttons[i] = buttonObj.GetComponent<NumericButton>();
            }

            // Create Clear button
            GameObject clearBtn = CreateButton(-1, new Vector3(-0.15f, -0.25f, 0), "ClearButton", true);
            clearBtn.transform.SetParent(buttonGrid.transform);

            // Create Submit button
            GameObject submitBtn = CreateButton(-2, new Vector3(0.15f, -0.25f, 0), "SubmitButton", true);
            submitBtn.transform.SetParent(buttonGrid.transform);

            // Wire up CodeInputPanel
            CodeInputPanel panel = GetComponent<CodeInputPanel>();
            if (panel == null)
            {
                panel = gameObject.AddComponent<CodeInputPanel>();
            }

            // Use reflection to set private fields (Unity Inspector will handle this normally)
            Debug.Log("Test setup created! Now manually assign in Inspector:");
            Debug.Log("- LED Display: Display GameObject");
            Debug.Log("- Timer: Timer GameObject");
            Debug.Log("- Numeric Buttons: 6 button GameObjects from ButtonGrid");
            Debug.Log("- Clear/Submit buttons");
        }

        private GameObject CreateButton(int value, Vector3 position, string name = "", bool isSpecial = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = $"Button_{value}";
            }

            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.localPosition = position;

            // Create visual
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "ButtonVisual";
            visual.transform.SetParent(buttonObj.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(0.08f, 0.02f, 0.08f);

            // Add label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(visual.transform);
            labelObj.transform.localPosition = new Vector3(0, 0.011f, 0);
            labelObj.transform.localRotation = Quaternion.Euler(90, 0, 0);

            var canvas = labelObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            labelObj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            labelObj.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

            var text = new GameObject("Text").AddComponent<TMPro.TextMeshProUGUI>();
            text.transform.SetParent(labelObj.transform);
            text.text = isSpecial ? (value == -1 ? "CLR" : "OK") : value.ToString();
            text.fontSize = 60;
            text.alignment = TMPro.TextAlignmentOptions.Center;
            text.color = Color.black;
            text.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            text.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // Add components
            BoxCollider collider = buttonObj.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.08f, 0.02f, 0.08f);

            var interactable = buttonObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

            NumericButton numButton = buttonObj.AddComponent<NumericButton>();

            return buttonObj;
        }
    }
}
