using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Core
{
    public class ScaleController : MonoBehaviour
    {
        [Header("Scale Configuration")]
        [SerializeField] private bool acceptsOnlyReferenceObjects = false;

        [Header("Component References")]
        [SerializeField] private WeightDisplay weightDisplay;

        [Header("Events")]
        public UnityEvent<float> OnWeightChanged;

        private HashSet<WeighableObject> objectsOnScale = new HashSet<WeighableObject>();
        private Dictionary<WeighableObject, XRGrabInteractable> grabbableCache = new Dictionary<WeighableObject, XRGrabInteractable>();

        public float CurrentWeight { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<WeighableObject>(out var weighableObj))
            {
                // FILTER: Check if this scale accepts this type of object
                if (weighableObj.IsReferenceObject != acceptsOnlyReferenceObjects)
                {
                    // Wrong type of object for this scale - ignore it
                    Debug.LogWarning($"[Scale] Objeto {other.name} no permitido en esta báscula (isReference={weighableObj.IsReferenceObject}, acceptsReference={acceptsOnlyReferenceObjects})");
                    return;
                }

                // Only add if has Rigidbody
                if (other.TryGetComponent<Rigidbody>(out var rb))
                {
                    objectsOnScale.Add(weighableObj);

                    // Cache XRGrabInteractable if present
                    if (weighableObj.TryGetComponent<XRGrabInteractable>(out var grabInteractable))
                    {
                        grabbableCache[weighableObj] = grabInteractable;
                        // Subscribe to events
                        grabInteractable.selectEntered.AddListener((args) => OnObjectGrabbed(weighableObj));
                        grabInteractable.selectExited.AddListener((args) => OnObjectReleased(weighableObj));
                    }

                    CalculateWeight();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<WeighableObject>(out var weighableObj))
            {
                objectsOnScale.Remove(weighableObj);

                // Clean up cached references and unsubscribe from events
                if (grabbableCache.TryGetValue(weighableObj, out var grabInteractable))
                {
                    grabInteractable.selectEntered.RemoveListener((args) => OnObjectGrabbed(weighableObj));
                    grabInteractable.selectExited.RemoveListener((args) => OnObjectReleased(weighableObj));
                    grabbableCache.Remove(weighableObj);
                }

                CalculateWeight();
            }
        }

        private void OnObjectGrabbed(WeighableObject obj)
        {
            // Immediately recalculate weight when object is grabbed
            CalculateWeight();
        }

        private void OnObjectReleased(WeighableObject obj)
        {
            // Recalculate weight when object is released
            CalculateWeight();
        }

        private void CalculateWeight()
        {
            float totalWeight = 0f;

            // Create list to track objects to remove (can't modify HashSet during iteration)
            List<WeighableObject> toRemove = new List<WeighableObject>();

            foreach (var obj in objectsOnScale)
            {
                if (obj == null)
                {
                    toRemove.Add(obj);
                    continue;
                }

                // Only count if NOT being held by player
                if (!IsObjectBeingHeld(obj))
                {
                    totalWeight += obj.Weight;
                }
            }

            // Clean up null references
            foreach (var obj in toRemove)
            {
                objectsOnScale.Remove(obj);
            }

            // Only update if weight actually changed
            if (!Mathf.Approximately(CurrentWeight, totalWeight))
            {
                CurrentWeight = totalWeight;
                weightDisplay?.UpdateWeight(totalWeight);
                OnWeightChanged?.Invoke(totalWeight);
            }
        }

        private bool IsObjectBeingHeld(WeighableObject obj)
        {
            if (grabbableCache.TryGetValue(obj, out XRGrabInteractable grabInteractable))
            {
                // XRGrabInteractable.isSelected returns true when object is grabbed
                return grabInteractable.isSelected;
            }

            // If no grab component, object can't be held
            return false;
        }
    }
}
