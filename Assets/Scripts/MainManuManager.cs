using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class MainManuManager : MonoBehaviour
    {
        [Header("Menu Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button exitButton;

        private void Start()
        {
            // Configurar listeners de los botones
            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayClicked);
            }

            if (exitButton != null)
            {
                exitButton.onClick.AddListener(OnExitClicked);
            }

            // Deshabilitar botones de Opciones y Créditos
            if (optionsButton != null)
            {
                optionsButton.interactable = false;
            }

            if (creditsButton != null)
            {
                creditsButton.interactable = false;
            }
        }

        private void OnPlayClicked()
        {
            // Cargar el nivel 1 usando el SceneLoader estático con pantalla de carga asíncrona
            SceneLoader.LoadSceneAsync("prototipo");
        }

        private void OnExitClicked()
        {
            // Cerrar la aplicación
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        private void OnDestroy()
        {
            // Limpiar listeners para evitar memory leaks
            if (playButton != null)
            {
                playButton.onClick.RemoveListener(OnPlayClicked);
            }

            if (exitButton != null)
            {
                exitButton.onClick.RemoveListener(OnExitClicked);
            }
        }
    }
}