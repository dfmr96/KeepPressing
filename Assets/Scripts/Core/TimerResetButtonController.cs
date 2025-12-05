using UnityEngine;

namespace Core
{
    public class TimerResetButtonController : MonoBehaviour
    {
        [SerializeField] private CountdownTimer countdownTimer;

        public void OnActivated()
        {
            countdownTimer?.ResetToDefault();
            Debug.Log("⏱ Timer reiniciado manualmente");
        }
    }
}
