using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Core
{
    /// <summary>
    /// Activates/deactivates GameObjects when an object is placed in or removed from an XR Socket Interactor
    /// </summary>
    public class SocketActivator : MonoBehaviour
    {
        [Header("GameObjects to Activate")]
        [SerializeField] private GameObject objectToActivate1;
        [SerializeField] private GameObject objectToActivate2;

        [Header("Configuration")]
        [SerializeField] private bool deactivateOnRemove = true;

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = false;

        private XRSocketInteractor socketInteractor;

        private void Start()
        {
            // Get the XR Socket Interactor component
            socketInteractor = GetComponent<XRSocketInteractor>();

            if (socketInteractor == null)
            {
                Debug.LogError("[SocketActivator] XRSocketInteractor component not found on this GameObject!");
                enabled = false;
                return;
            }

            // Subscribe to socket events
            socketInteractor.selectEntered.AddListener(OnObjectPlacedInSocket);
            socketInteractor.selectExited.AddListener(OnObjectRemovedFromSocket);

            // Initially deactivate the objects
            if (objectToActivate1 != null)
                objectToActivate1.SetActive(false);
            if (objectToActivate2 != null)
                objectToActivate2.SetActive(false);

            if (showDebugLogs)
            {
                Debug.Log($"[SocketActivator] Initialized on {gameObject.name}");
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            if (socketInteractor != null)
            {
                socketInteractor.selectEntered.RemoveListener(OnObjectPlacedInSocket);
                socketInteractor.selectExited.RemoveListener(OnObjectRemovedFromSocket);
            }
        }

        private void OnObjectPlacedInSocket(SelectEnterEventArgs args)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[SocketActivator] Object placed in socket: {args.interactableObject.transform.name}");
            }

            // Activate both GameObjects
            if (objectToActivate1 != null)
            {
                objectToActivate1.SetActive(true);
                if (showDebugLogs)
                    Debug.Log($"[SocketActivator] Activated: {objectToActivate1.name}");
            }

            if (objectToActivate2 != null)
            {
                objectToActivate2.SetActive(true);
                if (showDebugLogs)
                    Debug.Log($"[SocketActivator] Activated: {objectToActivate2.name}");
            }
        }

        private void OnObjectRemovedFromSocket(SelectExitEventArgs args)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[SocketActivator] Object removed from socket: {args.interactableObject.transform.name}");
            }

            // Deactivate both GameObjects if configured to do so
            if (deactivateOnRemove)
            {
                if (objectToActivate1 != null)
                {
                    objectToActivate1.SetActive(false);
                    if (showDebugLogs)
                        Debug.Log($"[SocketActivator] Deactivated: {objectToActivate1.name}");
                }

                if (objectToActivate2 != null)
                {
                    objectToActivate2.SetActive(false);
                    if (showDebugLogs)
                        Debug.Log($"[SocketActivator] Deactivated: {objectToActivate2.name}");
                }
            }
        }
    }
}
