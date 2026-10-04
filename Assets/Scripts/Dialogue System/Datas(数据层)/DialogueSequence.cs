using UnityEngine;
using System.Collections.Generic;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Sequence")]
    public class DialogueSequence : ScriptableObject
    {
        public string sequenceId;
        public List<DialogueLine> lines = new List<DialogueLine>();
    }
}