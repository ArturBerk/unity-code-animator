using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeAnimator
{
    public class CodeLine2 : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI LineNumberText;
        [SerializeField]
        private TextMeshProUGUI LineText;
        [SerializeField]
        private Graphic Background;

        public float LineNumberWidth = 70;
        public float Spacing = 10;

        public Color evenColor;
        public Color oddColor;
        
        [NonSerialized]
        public RectTransform RectTransform;

        private RectTransform rectLineNumberText;
        private RectTransform rectLineText;
        private float currentWidth;
        private int currentNumber = -1;
        private bool isInvalidated = true;
        
        private void Awake()
        {
            rectLineNumberText = (RectTransform) LineNumberText.transform;
            rectLineText = (RectTransform) LineText.transform;
            RectTransform = (RectTransform) transform;
            
            rectLineNumberText.sizeDelta = new Vector2(LineNumberWidth,0);
            rectLineNumberText.anchorMin = Vector2.up;
            rectLineNumberText.anchorMax = Vector2.up;
            rectLineNumberText.anchoredPosition = Vector2.zero;
            
            rectLineText.sizeDelta = new Vector2(LineNumberWidth,0);
            rectLineText.anchorMin = Vector2.up;
            rectLineText.anchorMax = Vector2.up;
            rectLineText.anchoredPosition = new Vector2(LineNumberWidth + Spacing,0);
        }
        
        public void SetLine(int number, string line)
        {
            if (StringComparer.Ordinal.Compare(LineText.text, line) == 0 && number == currentNumber) return;
            currentNumber = number;
            LineNumberText.text = number.ToString();
            LineText.text = line;
            isInvalidated = true;
            Background.color = number % 2 == 1 ? oddColor : evenColor;
        }

        public Vector2 Layout(float width)
        {
            if (currentWidth == width && !isInvalidated) return RectTransform.sizeDelta;
            currentWidth = width;
            
            var lineTextWidth = width - LineNumberWidth - Spacing;
            SetWidth(rectLineText, lineTextWidth);
            LineNumberText.ComputeMarginSize();
            var height = LineNumberText.preferredHeight;
            SetHeight(rectLineNumberText, height);
            SetHeight(rectLineText, height);

            RectTransform.sizeDelta = new Vector2(width, height);
            isInvalidated = false;
            return RectTransform.sizeDelta;
        }

        private void SetWidth(RectTransform rectTransform, float value)
        {
            if (Math.Abs(rectTransform.sizeDelta.x - value) > 0.001f)
            {
                rectTransform.sizeDelta = new Vector2(value, rectTransform.sizeDelta.y);
            }
        }
        
        private void SetHeight(RectTransform rectTransform, float value)
        {
            if (Math.Abs(rectTransform.sizeDelta.y - value) > 0.001f)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, value);
            }
        }
    }
}