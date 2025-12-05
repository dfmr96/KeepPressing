using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class PuzzleCode
    {
        [SerializeField] private string codeName = "Código 1";
        [SerializeField] private string codeValue = "1234";

        private bool hasBeenUsed = false;

        public string CodeName => codeName;
        public string CodeValue => codeValue;
        public bool HasBeenUsed => hasBeenUsed;

        public bool Matches(string input)
        {
            return codeValue == input;
        }

        public void MarkAsUsed()
        {
            hasBeenUsed = true;
            Debug.Log($"✔ [{codeName}] Marcado como usado");
        }
    }
}
