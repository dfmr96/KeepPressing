using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;

public class VRFadeManager : MonoBehaviour
{
    public static VRFadeManager Instance { get; private set; }

    [Header("Tunneling Vignette")]
    [Tooltip("Referencia al TunnelingVignetteController del XR Origin (se busca automáticamente si está vacío)")]
    [SerializeField] private TunnelingVignetteController vignetteController;

    [Header("Fade Settings")]
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private Color fadeColor = Color.black;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private FadeVignetteProvider currentProvider;
    private bool isFading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        FindVignetteController();
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (debugMode) Debug.Log($"[VRFadeManager] Escena cargada: {scene.name}. Buscando VignetteController de nuevo.");

        // Buscar el controller de nuevo
        FindVignetteController();

        // Si hay un provider activo, aplicarlo INSTANTÁNEAMENTE al nuevo controller
        if (currentProvider != null && vignetteController != null)
        {
            if (debugMode) Debug.Log("[VRFadeManager] Aplicando viñeta cerrada instantáneamente en la nueva escena.");

            // Crear provider con cierre instantáneo (easeInTime = 0)
            var instantParameters = new VignetteParameters
            {
                apertureSize = 0f,
                featheringEffect = 0.1f,
                vignetteColor = fadeColor,
                vignetteColorBlend = fadeColor,
                apertureVerticalPosition = 0f,
                easeInTime = 0f, // Cerrar instantáneamente
                easeOutTime = fadeInDuration
            };

            var instantProvider = new FadeVignetteProvider(instantParameters);

            // Aplicar instantáneamente - la escena nueva inicia con viñeta cerrada
            vignetteController.BeginTunnelingVignette(instantProvider);

            // Actualizar currentProvider para que FadeIn pueda usarlo
            currentProvider = instantProvider;

            if (debugMode) Debug.Log("[VRFadeManager] Viñeta cerrada lista - esperando FadeIn.");
        }
    }

    private void FindVignetteController()
    {
        // Siempre buscar de nuevo en caso de que haya cambiado de escena
        vignetteController = FindObjectOfType<TunnelingVignetteController>();

        if (vignetteController == null)
        {
            Debug.LogError("[VRFadeManager] No se encontró TunnelingVignetteController en la escena. Asegúrate de que el XR Origin tenga un TunnelingVignette como hijo de la Main Camera.");
        }
        else
        {
            if (debugMode) Debug.Log($"[VRFadeManager] TunnelingVignetteController encontrado correctamente: {vignetteController.gameObject.name}");
        }
    }

    private void EnsureVignetteController()
    {
        // Verificar si la referencia es válida antes de usarla
        if (vignetteController == null)
        {
            if (debugMode) Debug.LogWarning("[VRFadeManager] VignetteController es null, buscando de nuevo...");
            FindVignetteController();
        }
    }

    /// <summary>
    /// Fade out (cierra la visión a negro tipo túnel)
    /// </summary>
    public IEnumerator FadeOut()
    {
        // Asegurar que tenemos referencia válida al controller
        EnsureVignetteController();

        if (vignetteController == null)
        {
            if (debugMode) Debug.LogWarning("[VRFadeManager] FadeOut: vignetteController es null después de EnsureVignetteController.");
            yield break;
        }

        if (isFading)
        {
            if (debugMode) Debug.LogWarning("[VRFadeManager] FadeOut: Ya hay un fade en progreso.");
            yield break;
        }

        isFading = true;
        if (debugMode) Debug.Log($"[VRFadeManager] Iniciando FadeOut (duración: {fadeOutDuration}s)");

        // Crear proveedor con parámetros para fade out completo
        var parameters = new VignetteParameters
        {
            apertureSize = 0f, // Cerrado completo (pantalla negra)
            featheringEffect = 0.1f, // Poco difuminado para transición limpia
            vignetteColor = fadeColor,
            vignetteColorBlend = fadeColor,
            apertureVerticalPosition = 0f,
            easeInTime = fadeOutDuration, // Tiempo de transición
            easeOutTime = fadeInDuration
        };

        currentProvider = new FadeVignetteProvider(parameters);
        vignetteController.BeginTunnelingVignette(currentProvider);

        if (debugMode) Debug.Log("[VRFadeManager] BeginTunnelingVignette llamado.");

        yield return new WaitForSeconds(fadeOutDuration);

        isFading = false;
        if (debugMode) Debug.Log("[VRFadeManager] FadeOut completado.");
    }

    /// <summary>
    /// Fade in (abre la visión desde negro)
    /// </summary>
    public IEnumerator FadeIn()
    {
        // Asegurar que tenemos referencia válida al controller
        EnsureVignetteController();

        if (vignetteController == null)
        {
            if (debugMode) Debug.LogWarning("[VRFadeManager] FadeIn: vignetteController es null después de EnsureVignetteController.");
            yield break;
        }

        if (isFading)
        {
            if (debugMode) Debug.LogWarning("[VRFadeManager] FadeIn: Ya hay un fade en progreso.");
            yield break;
        }

        isFading = true;
        if (debugMode) Debug.Log($"[VRFadeManager] Iniciando FadeIn (duración: {fadeInDuration}s)");

        // La viñeta ya está cerrada (OnSceneLoaded la aplicó instantáneamente)
        // Solo necesitamos abrirla
        if (currentProvider != null)
        {
            if (debugMode) Debug.Log("[VRFadeManager] Abriendo viñeta desde estado cerrado.");

            // Abrir la viñeta (usa el easeOutTime del provider)
            vignetteController.EndTunnelingVignette(currentProvider);

            if (debugMode) Debug.Log("[VRFadeManager] EndTunnelingVignette ejecutado - apertura animada iniciada.");

            // Esperar a que termine la animación de apertura
            yield return new WaitForSeconds(fadeInDuration);

            // Limpiar el currentProvider
            currentProvider = null;

            if (debugMode) Debug.Log("[VRFadeManager] FadeIn completado - viñeta completamente abierta.");
        }
        else
        {
            if (debugMode) Debug.LogWarning("[VRFadeManager] FadeIn: No hay currentProvider activo. La viñeta podría no abrirse.");
        }

        isFading = false;
    }

    /// <summary>
    /// Proveedor personalizado para el fade
    /// </summary>
    private class FadeVignetteProvider : ITunnelingVignetteProvider
    {
        private VignetteParameters m_Parameters;

        public FadeVignetteProvider(VignetteParameters parameters)
        {
            m_Parameters = parameters;
        }

        public VignetteParameters vignetteParameters => m_Parameters;

        public LocomotionVignetteProvider locomotionVignetteProvider => null;
    }
}
