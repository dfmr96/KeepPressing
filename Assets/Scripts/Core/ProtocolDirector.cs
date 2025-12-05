using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class ProtocolDirector : MonoBehaviour
    {
        [Header("Timer settings")]
        [SerializeField] private float defaultTime = 120f;
        [SerializeField] private float reducedTime = 60f;

        [Header("Puzzle Codes")]
        [SerializeField] private PuzzleCode[] puzzleCodes = new PuzzleCode[3];

        [Header("Sphere Activation")]
        [SerializeField] private GameObject[] spheres = new GameObject[3];
        [SerializeField] private Material activeMaterial;

        [Header("Component References")]
        [SerializeField] private CountdownTimer countdownTimer;

        [Header("Events")]
        public UnityEvent OnCodeCorrect;
        public UnityEvent OnCodeIncorrect;
        public UnityEvent OnTimeExpired;

        public float GetDefaultTime() => defaultTime;
        public float GetReducedTime() => reducedTime;

        public string GetCorrectCode()
        {
            return puzzleCodes.Length > 0 && puzzleCodes[0] != null
                ? puzzleCodes[0].CodeValue
                : "1234";
        }

        public int ValidateCode(string enteredCode, out string usedCodeName)
        {
            for (int i = 0; i < puzzleCodes.Length; i++)
            {
                if (puzzleCodes[i] != null && puzzleCodes[i].Matches(enteredCode))
                {
                    usedCodeName = puzzleCodes[i].CodeName;
                    return i;
                }
            }

            usedCodeName = null;
            return -1;
        }

        public void OnCorrectCodeEntered(int codeIndex, string codeName)
        {
            Debug.Log($"✔ [Director] {codeName} utilizado");

            if (codeIndex >= 0 && codeIndex < puzzleCodes.Length)
            {
                if (puzzleCodes[codeIndex] != null && puzzleCodes[codeIndex].HasBeenUsed)
                {
                    Debug.Log($"⚠ [Director] {codeName} ya fue utilizado anteriormente");
                    return;
                }

                if (puzzleCodes[codeIndex] != null)
                {
                    puzzleCodes[codeIndex].MarkAsUsed();
                }

                ActivateNextSphere();
            }

            OnCodeCorrect?.Invoke();
        }

        private void ActivateNextSphere()
        {
            if (activeMaterial == null)
            {
                Debug.LogWarning("[Director] No se asignó material activo");
                return;
            }

            foreach (GameObject sphere in spheres)
            {
                if (sphere == null) continue;

                MeshRenderer renderer = sphere.GetComponent<MeshRenderer>();
                if (renderer == null) continue;

                if (renderer.sharedMaterial != activeMaterial)
                {
                    renderer.material = activeMaterial;
                    Debug.Log($"✔ [Director] Esfera {sphere.name} activada");
                    return;
                }
            }

            Debug.Log("⚠ [Director] Todas las esferas ya están activadas");
        }

        public void OnIncorrectCodeEntered()
        {
            Debug.Log("✘ [Director] Código incorrecto");
            OnCodeIncorrect?.Invoke();
        }

        public void OnTimerExpired()
        {
            Debug.Log("⏱ [Director] Tiempo agotado - GAME OVER");
            OnTimeExpired?.Invoke();
        }
    }
}