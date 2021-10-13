using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeAnimator
{
    public class CodeLine : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private TextMeshProUGUI lineNumberText;

        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private Graphic background;
        [SerializeField]
        private Graphic newBackground;

        [SerializeField]
        private float FadeDuration = 5;

        private bool isNew;

            [NonSerialized]
        public RectTransform RectTransform;
        public ILayoutElement LayoutElement;
        private int number = -1;
        private Vector2 position = Vector2.negativeInfinity;

        [SerializeField]
        private Color evenColor;
        [SerializeField]
        private Color oddColor;

        [SerializeField] 
        private float colorChangeDuration;

        private void Awake()
        {
            RectTransform = (RectTransform) transform;
            LayoutElement = GetComponent<ILayoutElement>();
            isNew = true;

            newBackground.DOFade(0, FadeDuration).SetEase(Ease.OutSine).OnComplete(() =>
            {
                Destroy(newBackground);
            });
        }

        [NonSerialized]
        public string OriginalLine;
        
        public string Text
        {
            get => text.text;
            set => text.text = value;
        }

        public int Number
        {
            get => number;
            set
            {
                if (number == value) return;
                var wasEven = number % 2 == 0;
                number = value;
                lineNumberText.text = number.ToString();
                var newEven = number % 2 == 0;
                if (newEven != wasEven || isNew)
                {
                    background.DOColor(newEven ? evenColor : oddColor, colorChangeDuration);
                }
            }
        }

        public Vector2 Position
        {
            get => position;
            set
            {
                if (Vector2.Distance(position, value) < 0.001f)
                {
                    position = value;
                    RectTransform.anchoredPosition = value;
                    return;
                }
                position = value;

                RectTransform.DOKill();
                if (isNew)
                {
                    canvasGroup.alpha = 0;
                    isNew = false;
                    RectTransform.anchoredPosition = position + new Vector2(100, 0);
                    
                    var sequence = DOTween.Sequence();
                    sequence.Insert(0.25f, RectTransform.DOAnchorPos(value, 0.25f).SetEase(Ease.OutSine));
                    sequence.Insert(0.25f, canvasGroup.DOFade(1, 0.25f));
                    sequence.SetTarget(RectTransform);
                    sequence.Play();
                }
                else
                {
                    canvasGroup.alpha = 1;
                    RectTransform.DOAnchorPos(value, 0.25f).SetEase(Ease.OutSine);
                }

            }
        }

        public void Remove()
        {
            RectTransform.DOKill();
            canvasGroup.DOFade(0, 0.25f).SetEase(Ease.InSine);
            RectTransform
                .DOAnchorPosX(RectTransform.anchoredPosition.x - 100, 0.25f)
                .SetEase(Ease.InSine)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}