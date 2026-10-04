using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace DialogueSystem
{
    public class DialogueUIView : MonoBehaviour
    {
        [Header("整体显隐")]
        [Tooltip("UI 根物体上的 CanvasGroup，控制整体显示/隐藏")]
        public CanvasGroup rootGroup;

        [Header("文本")]
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI contentText;
        public GameObject namePanel;

        [Header("立绘")]
        public Image leftPortrait;
        public Image rightPortrait;

        [Header("对话框")]
        public CanvasGroup dialogueBoxGroup;
        public GameObject continueIcon;

        [Header("立绘表现")]
        [Range(0f, 1f)] public float dimFactor = 0.45f;
        public float portraitFadeSpeed = 6f;

        // 左右当前角色
        private DialogueCharacter _leftChar;
        private DialogueCharacter _rightChar;

        // 协程句柄，防止叠加
        private Coroutine _leftFadeCo;
        private Coroutine _rightFadeCo;

        // ---------- 生命周期 ----------

        private void Awake()
        {
            // 初始隐藏整个对话 UI
            if (rootGroup != null)
            {
                rootGroup.alpha = 0f;
                rootGroup.blocksRaycasts = false;
                rootGroup.interactable = false;
            }
        }

        // ---------- 对外接口 ----------

        public void ShowLine(DialogueLine line)
        {
            // 整体显示
            if (rootGroup != null)
            {
                rootGroup.alpha = 1f;
                rootGroup.blocksRaycasts = true;
                rootGroup.interactable = true;
            }

            // 姓名
            if (line.IsNarration)
            {
                namePanel.SetActive(false);
            }
            else
            {
                namePanel.SetActive(true);
                nameText.text = line.speaker.characterName;
                nameText.color = line.speaker.nameColor;
            }

            // 正文
            contentText.text = line.text;
            contentText.maxVisibleCharacters = 0;

            // 立绘
            UpdatePortraits(line);

            // 对话框淡入
            dialogueBoxGroup.alpha = 1f;
        }

        public void SetVisibleCharacters(int count)
        {
            contentText.maxVisibleCharacters = count;
        }

        public void SetContinueIcon(bool visible)
        {
            if (continueIcon != null)
                continueIcon.SetActive(visible);
        }

        public void Hide()
        {
            // 对话框淡出
            dialogueBoxGroup.alpha = 0f;
            SetContinueIcon(false);

            // 立绘淡出
            if (_leftChar != null) StartFade(leftPortrait, 0f, isLeft: true, fadeAlpha: true);
            if (_rightChar != null) StartFade(rightPortrait, 0f, isLeft: false, fadeAlpha: true);

            // 整体隐藏
            if (rootGroup != null)
            {
                rootGroup.alpha = 0f;
                rootGroup.blocksRaycasts = false;
                rootGroup.interactable = false;
            }
        }

        // ---------- 内部逻辑 ----------

        private void UpdatePortraits(DialogueLine line)
        {
            if (line.speaker == null) return;

            if (line.side == PortraitSide.Left)
                UpdateOneSide(line, isLeft: true);
            else
                UpdateOneSide(line, isLeft: false);
        }

        private void UpdateOneSide(DialogueLine line, bool isLeft)
        {
            Image targetImg = isLeft ? leftPortrait : rightPortrait;
            Image oppositeImg = isLeft ? rightPortrait : leftPortrait;

            DialogueCharacter currentChar = isLeft ? _leftChar : _rightChar;
            DialogueCharacter oppositeChar = isLeft ? _rightChar : _leftChar;

            // 1) 目标侧
            if (currentChar != line.speaker)
            {
                // 换角色 → 先设 sprite 再淡入
                if (isLeft) _leftChar = line.speaker;
                else _rightChar = line.speaker;

                targetImg.gameObject.SetActive(true);
                targetImg.sprite = line.speaker.GetPortrait(line.expression);

                var c = targetImg.color;
                c.a = 0f;
                targetImg.color = c;
                StartFade(targetImg, 1f, isLeft);
            }
            else
            {
                // 同角色 → 换表情，不重播淡入（但要保证它是激活的）
                targetImg.gameObject.SetActive(true);
                targetImg.sprite = line.speaker.GetPortrait(line.expression);
                StartFade(targetImg, 1f, isLeft, fadeAlpha: false);
            }

            // 2) 非说话侧压暗
            if (oppositeChar != null)
            {
                StartFade(oppositeImg, dimFactor, isLeft: !isLeft, fadeAlpha: false);
            }
        }

        private void StartFade(Image img, float targetValue, bool isLeft, bool fadeAlpha = true)
        {
            if (isLeft && _leftFadeCo != null) StopCoroutine(_leftFadeCo);
            if (!isLeft && _rightFadeCo != null) StopCoroutine(_rightFadeCo);

            var co = StartCoroutine(FadeRoutine(img, targetValue, fadeAlpha));
            if (isLeft) _leftFadeCo = co;
            else _rightFadeCo = co;
        }

        private IEnumerator FadeRoutine(Image img, float targetValue, bool fadeAlpha)
        {
            Color c = img.color;

            float startA = c.a;
            float startRGB = c.r;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * portraitFadeSpeed;
                float lerp = Mathf.Clamp01(t);

                if (fadeAlpha)
                {
                    c.a = Mathf.Lerp(startA, targetValue, lerp);
                }
                else
                {
                    float v = Mathf.Lerp(startRGB, targetValue, lerp);
                    c.r = c.g = c.b = v;
                    c.a = 1f;
                }

                img.color = c;
                yield return null;
            }

            if (fadeAlpha)
            {
                c.a = targetValue;
                if (Mathf.Approximately(targetValue, 0f))
                    img.gameObject.SetActive(false);
            }
            else
            {
                c.r = c.g = c.b = targetValue;
                c.a = 1f;
            }
            img.color = c;
        }
    }
}