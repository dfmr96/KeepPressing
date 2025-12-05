using UnityEngine;

namespace Core
{
    public class WeighableObject : MonoBehaviour
    {
        [SerializeField] private float weight = 1.0f;
        [SerializeField] private bool isReferenceObject = false;

        public float Weight => weight;
        public bool IsReferenceObject => isReferenceObject;
    }
}
