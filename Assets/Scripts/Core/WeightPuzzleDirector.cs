using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class WeightPuzzleDirector : MonoBehaviour
    {
        [Header("Scale References")]
        [SerializeField] private ScaleController referenceScale;
        [SerializeField] private ScaleController playerScale;

        [Header("Events")]
        public UnityEvent OnWeightsMatch;

        private bool hasMatchedOnce = false;

        private void Start()
        {
            // Subscribe to weight changes from both scales
            if (referenceScale != null)
            {
                referenceScale.OnWeightChanged.AddListener(OnAnyWeightChanged);
            }

            if (playerScale != null)
            {
                playerScale.OnWeightChanged.AddListener(OnAnyWeightChanged);
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            if (referenceScale != null)
            {
                referenceScale.OnWeightChanged.RemoveListener(OnAnyWeightChanged);
            }

            if (playerScale != null)
            {
                playerScale.OnWeightChanged.RemoveListener(OnAnyWeightChanged);
            }
        }

        private void OnAnyWeightChanged(float weight)
        {
            // Check weights whenever either scale changes
            CheckWeights();
        }

        private void CheckWeights()
        {
            if (referenceScale == null || playerScale == null)
            {
                Debug.LogWarning("[WeightPuzzle] Reference scale or player scale not assigned!");
                return;
            }

            bool weightsMatch = Mathf.Approximately(
                referenceScale.CurrentWeight,
                playerScale.CurrentWeight
            );

            if (weightsMatch && !hasMatchedOnce)
            {
                hasMatchedOnce = true;
                Debug.Log($"✔ [WeightPuzzle] ¡Pesos coinciden! Reference: {referenceScale.CurrentWeight:F2} kg, Player: {playerScale.CurrentWeight:F2} kg");
                OnWeightsMatch?.Invoke();
            }
            else if (!weightsMatch)
            {
                Debug.Log($"[WeightPuzzle] Pesos no coinciden. Reference: {referenceScale.CurrentWeight:F2} kg, Player: {playerScale.CurrentWeight:F2} kg");
            }
        }

        // Public method to reset the puzzle
        public void ResetPuzzle()
        {
            hasMatchedOnce = false;
            Debug.Log("[WeightPuzzle] Puzzle reseteado");
        }
    }
}
