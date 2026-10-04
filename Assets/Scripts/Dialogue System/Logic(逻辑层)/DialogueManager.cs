using UnityEngine;
using System;
using System.Collections;

namespace DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("引用")]
        public DialogueUIView uiView;
        public TypewriterEffect typewriter;

        [Header("全局设置")]
        public float defaultTypingSpeed = 30f;
        public bool allowSkipTyping = true;

        public event Action OnDialogueStart;
        public event Action OnDialogueEnd;

        private DialogueSequence _currentSequence;
        private int _currentIndex;
        private bool _isActive;

        public bool IsActive => _isActive;
        public bool IsTyping => typewriter != null && typewriter.IsTyping;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (typewriter != null)
                typewriter.OnTypingComplete += HandleTypingComplete;
        }

        private void OnDestroy()
        {
            if (typewriter != null)
                typewriter.OnTypingComplete -= HandleTypingComplete;
        }

        public void StartDialogue(DialogueSequence sequence)
        {
            if (sequence == null || sequence.lines.Count == 0) return;

            _currentSequence = sequence;
            _currentIndex = 0;
            _isActive = true;
            OnDialogueStart?.Invoke();
            ShowCurrentLine();
        }

        public void Advance()
        {
            if (!_isActive) return;

            if (IsTyping)
            {
                if (allowSkipTyping) typewriter.CompleteImmediately();
                return;
            }

            _currentIndex++;
            if (_currentIndex >= _currentSequence.lines.Count)
            {
                EndDialogue();
                return;
            }
            ShowCurrentLine();
        }

        private void ShowCurrentLine()
        {
            var line = _currentSequence.lines[_currentIndex];
            uiView.ShowLine(line);
            uiView.SetContinueIcon(false);

            float speed = line.typingSpeed > 0 ? line.typingSpeed : defaultTypingSpeed;
            typewriter.Play(line.text, speed);
        }

        private void HandleTypingComplete()
        {
            uiView.SetContinueIcon(true);

            var line = _currentSequence.lines[_currentIndex];
            if (line.autoAdvanceDelay > 0f)
                StartCoroutine(AutoAdvanceRoutine(line.autoAdvanceDelay));
        }

        private IEnumerator AutoAdvanceRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (_isActive && !IsTyping) Advance();
        }

        private void EndDialogue()
        {
            _isActive = false;
            uiView.Hide();
            OnDialogueEnd?.Invoke();
        }
    }
}
/*
 * 外部调用
 using UnityEngine;
using DialogueSystem;   // 关键：一行搞定

public class NpcTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSequence sequence;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            DialogueManager.Instance.StartDialogue(sequence);
    }
}
 */