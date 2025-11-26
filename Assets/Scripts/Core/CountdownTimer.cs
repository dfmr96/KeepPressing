using UnityEngine;
using UnityEngine.Events;

namespace KeepPressing.Core
{
    /// <summary>
    /// Countdown timer system for the code input protocol.
    /// Supports configurable duration and fires events at key moments.
    /// </summary>
    public class CountdownTimer : MonoBehaviour
    {
        [Header("Timer Settings")]
        [SerializeField] private float timerDuration = 120f;
        [SerializeField] private float warningThreshold = 10f;

        [Header("Events")]
        public UnityEvent OnTimerStart;
        public UnityEvent<float> OnTimerTick;
        public UnityEvent OnTimerWarning;
        public UnityEvent OnTimerExpired;

        private float currentTime;
        private bool isRunning;
        private bool warningTriggered;

        public float CurrentTime => currentTime;
        public float Duration => timerDuration;
        public bool IsRunning => isRunning;
        public float TimeRemaining => Mathf.Max(0, currentTime);
        public float TimeElapsed => timerDuration - currentTime;
        public float Progress => 1f - (currentTime / timerDuration);

        private void Start()
        {
            ResetTimer();
        }

        private void Update()
        {
            if (!isRunning) return;

            currentTime -= Time.deltaTime;

            OnTimerTick?.Invoke(currentTime);

            if (!warningTriggered && currentTime <= warningThreshold && currentTime > 0)
            {
                warningTriggered = true;
                OnTimerWarning?.Invoke();
            }

            if (currentTime <= 0)
            {
                currentTime = 0;
                isRunning = false;
                OnTimerExpired?.Invoke();
            }
        }

        /// <summary>
        /// Starts the countdown timer
        /// </summary>
        public void StartTimer()
        {
            isRunning = true;
            warningTriggered = false;
            OnTimerStart?.Invoke();
        }

        /// <summary>
        /// Pauses the countdown timer
        /// </summary>
        public void PauseTimer()
        {
            isRunning = false;
        }

        /// <summary>
        /// Resumes the countdown timer
        /// </summary>
        public void ResumeTimer()
        {
            isRunning = true;
        }

        /// <summary>
        /// Resets the timer to its initial duration
        /// </summary>
        public void ResetTimer()
        {
            currentTime = timerDuration;
            warningTriggered = false;
            isRunning = false;
        }

        /// <summary>
        /// Resets and immediately starts the timer
        /// </summary>
        public void RestartTimer()
        {
            ResetTimer();
            StartTimer();
        }

        /// <summary>
        /// Sets a new timer duration (useful for switching from 120s to 60s)
        /// </summary>
        /// <param name="newDuration">New duration in seconds</param>
        public void SetDuration(float newDuration)
        {
            timerDuration = newDuration;
            if (!isRunning)
            {
                currentTime = timerDuration;
            }
        }

        /// <summary>
        /// Returns time remaining formatted as MM:SS
        /// </summary>
        public string GetFormattedTime()
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }
    }
}
