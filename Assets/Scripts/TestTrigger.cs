using DialogueSystem;
using UnityEngine;

using UnityEngine;
using DialogueSystem;

public class TestTrigger : MonoBehaviour
{
    public DialogueSequence sequence;

    private void Start()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.StartDialogue(sequence);
    }
}