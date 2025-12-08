using UnityEngine;

/// <summary>
/// Script de debug para probar el sistema de fade VR
/// </summary>
public class VRFadeDebugger : MonoBehaviour
{
    [Header("Debug Controls")]
    [Tooltip("Presiona esta tecla para ejecutar FadeOut")]
    [SerializeField] private KeyCode fadeOutKey = KeyCode.F1;

    [Tooltip("Presiona esta tecla para ejecutar FadeIn")]
    [SerializeField] private KeyCode fadeInKey = KeyCode.F2;

    [Header("Status")]
    [SerializeField] private bool showGUI = true;

    private void Update()
    {
        // FadeOut con F1
        if (Input.GetKeyDown(fadeOutKey))
        {
            Debug.Log("[VRFadeDebugger] Ejecutando FadeOut...");
            if (VRFadeManager.Instance != null)
            {
                StartCoroutine(VRFadeManager.Instance.FadeOut());
            }
            else
            {
                Debug.LogError("[VRFadeDebugger] VRFadeManager.Instance es null!");
            }
        }

        // FadeIn con F2
        if (Input.GetKeyDown(fadeInKey))
        {
            Debug.Log("[VRFadeDebugger] Ejecutando FadeIn...");
            if (VRFadeManager.Instance != null)
            {
                StartCoroutine(VRFadeManager.Instance.FadeIn());
            }
            else
            {
                Debug.LogError("[VRFadeDebugger] VRFadeManager.Instance es null!");
            }
        }
    }

    private void OnGUI()
    {
        if (!showGUI) return;

        // Panel de información
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Box("VR Fade Debugger");

        GUILayout.Label($"FadeOut Key: {fadeOutKey}");
        GUILayout.Label($"FadeIn Key: {fadeInKey}");

        GUILayout.Space(10);

        if (VRFadeManager.Instance != null)
        {
            GUILayout.Label("✓ VRFadeManager: OK");
        }
        else
        {
            GUILayout.Label("✗ VRFadeManager: NULL");
        }

        GUILayout.Space(10);

        // Botones en pantalla
        if (GUILayout.Button("Execute FadeOut", GUILayout.Height(40)))
        {
            Debug.Log("[VRFadeDebugger] Botón FadeOut presionado");
            if (VRFadeManager.Instance != null)
            {
                StartCoroutine(VRFadeManager.Instance.FadeOut());
            }
        }

        if (GUILayout.Button("Execute FadeIn", GUILayout.Height(40)))
        {
            Debug.Log("[VRFadeDebugger] Botón FadeIn presionado");
            if (VRFadeManager.Instance != null)
            {
                StartCoroutine(VRFadeManager.Instance.FadeIn());
            }
        }

        GUILayout.EndArea();
    }
}
