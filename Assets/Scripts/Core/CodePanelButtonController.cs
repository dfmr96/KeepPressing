using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Core
{
    public class CodePanelButtonController : MonoBehaviour
    {
        [SerializeField] private int buttonValue;
        [SerializeField] private CodeDisplay codeDisplay;
        
        public void OnActivated()
        {
            codeDisplay.AddDigit(buttonValue);
        }
    }
}