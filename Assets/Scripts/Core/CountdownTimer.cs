using TMPro;
using UnityEngine;

namespace Core
{
    public class CountdownTimer : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private ProtocolDirector director;

        private float currentTime;
        private bool running;
        private bool reducedTime = false;

        private void Start()
        {
            currentTime = director.GetDefaultTime();
            running = true;
        }

        private void Update()
        {
            if (!running) return;

            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                running = false;
                Debug.Log("⏱ Tiempo agotado");
                director?.OnTimerExpired();
            }

            UpdateDisplay();
        }

        public void ResetToDefault()
        {
            if (!reducedTime)
            {
                currentTime = director.GetDefaultTime();
            }
            else
            {
                currentTime = director.GetReducedTime();
            }
            running = true;
        }

        public void SwitchToReducedTime()
        {
            currentTime = director.GetReducedTime();
            running = true;
            reducedTime = true;
        }

        private void UpdateDisplay()
        {
            int min = Mathf.FloorToInt(currentTime / 60);
            int sec = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"{min:00}:{sec:00}";
        }
    }
}