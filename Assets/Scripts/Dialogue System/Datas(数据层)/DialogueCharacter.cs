using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "Dialogue/Character")]
    public class DialogueCharacter : ScriptableObject
    {
        [Header("基础信息")]
        public string characterId;
        public string characterName;

        [Header("立绘配置")]
        public Sprite defaultPortrait;
        public CharacterExpression[] expressions;

        [Header("显示样式")]
        public Color nameColor = Color.white;

        public Sprite GetPortrait(string expressionName)
        {
            if (expressions == null || string.IsNullOrEmpty(expressionName))
                return defaultPortrait;

            foreach (var exp in expressions)
            {
                if (exp.expressionName == expressionName)
                    return exp.portrait != null ? exp.portrait : defaultPortrait;
            }
            return defaultPortrait;
        }
    }

    [System.Serializable]
    public class CharacterExpression
    {
        public string expressionName;
        public Sprite portrait;
    }
}