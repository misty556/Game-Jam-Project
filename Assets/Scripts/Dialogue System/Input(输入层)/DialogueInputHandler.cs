using UnityEngine;

namespace DialogueSystem
{
    public class DialogueInputHandler : MonoBehaviour
    {
        [SerializeField] private DialogueManager manager;

        private void Update()
        {
            if (manager == null || !manager.IsActive) return;

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                manager.Advance();
        }
    }
}