using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// 角色站位
    /// </summary>
    public enum PortraitSide
    {
        Left,
        Right
    }

    [System.Serializable]
    public class DialogueLine
    {
        [Header("说话人")]
        public DialogueCharacter speaker;
        public string expression = "normal";

        [Tooltip("说话人站在哪一侧")]
        public PortraitSide side = PortraitSide.Right;

        [Header("文本内容")]
        [TextArea(3, 8)]
        public string text;

        [Header("演出参数")]
        public float typingSpeed = 0f;
        public float autoAdvanceDelay = 0f;

        public bool IsNarration => speaker == null;
    }
}