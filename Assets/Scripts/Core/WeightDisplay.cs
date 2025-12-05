using TMPro;
using UnityEngine;

namespace Core
{
    public class WeightDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text weightText;
        [SerializeField] private string format = "{0:F2} kg";

        private void Start()
        {
            UpdateWeight(0f);
        }

        public void UpdateWeight(float weight)
        {
            if (weightText != null)
            {
                weightText.text = string.Format(format, weight);
            }
        }
    }
}
