using UnityEngine;
using TMPro;
using System;

namespace DialogueSystem
{
    public class TypewriterEffect : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI targetText;
        [SerializeField] private float defaultCharsPerSecond = 30f;

        private float _timer;
        private float _charsPerSecond;
        private int _totalChars;
        private bool _isTyping;

        public bool IsTyping => _isTyping;
        public event Action OnTypingComplete;

        public void Play(string text, float charsPerSecond = 0f)
        {
            targetText.text = text;
            targetText.maxVisibleCharacters = 0;
            targetText.ForceMeshUpdate();

            _totalChars = targetText.textInfo.characterCount;
            _charsPerSecond = charsPerSecond > 0 ? charsPerSecond : defaultCharsPerSecond;
            _timer = 0f;
            _isTyping = true;
        }

        public void CompleteImmediately()
        {
            if (!_isTyping) return;
            targetText.maxVisibleCharacters = _totalChars;
            _isTyping = false;
            OnTypingComplete?.Invoke();
        }

        private void Update()
        {
            if (!_isTyping) return;

            _timer += Time.deltaTime * _charsPerSecond;
            int visible = Mathf.Min((int)_timer, _totalChars);
            targetText.maxVisibleCharacters = visible;

            if (visible >= _totalChars)
            {
                _isTyping = false;
                OnTypingComplete?.Invoke();
            }
        }
    }
}