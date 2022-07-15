using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

namespace CodeAnimator
{
    [RequireComponent(typeof(RectTransform))]
    public class VerticalGroup : MonoBehaviour
    {
        [SerializeField]
        private float leftmargin;
        [SerializeField]
        private float rightmargin;
        
        public void Layout()
        {
            var rectTransform = (RectTransform)transform;
            var y = 0.0f;
            var rectWidth = rectTransform.rect.width - (leftmargin + rightmargin);
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                var child = (RectTransform)rectTransform.GetChild(i);
                
                var layout = child.GetComponent<ILayoutElement>();

                child.sizeDelta = new Vector2(rectWidth, 0);
                layout.CalculateLayoutInputHorizontal();
                //child.sizeDelta = new Vector2(rectTransform.rect.x, 0);
                layout.CalculateLayoutInputVertical();
                
                var height = LayoutUtility.GetPreferredHeight(child);
                child.sizeDelta = new Vector2(rectWidth, height);
                
                child.anchorMin = Vector2.up;
                child.anchorMax = Vector2.up;
                
                SetPosition(child, new Vector2(leftmargin, y));
                
                y -= height;
            }
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, -y);
        }

        private void SetPosition(RectTransform rectTransform, Vector2 position)
        {
            if (animations.TryGetValue(rectTransform, out var previousCoroutine)) StopCoroutine(previousCoroutine);
            animations[rectTransform] = StartCoroutine(Animate(rectTransform, position, 1));
        }

        private IEnumerator Animate(RectTransform rectTransform, Vector2 position, float duration)
        {
            var startPosition = rectTransform.anchoredPosition;
            var t = 0.0f;
            while (t < 1)
            {
                var easing = Easing.InOutSine(t);
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, position, easing);
                t += Time.deltaTime / duration;
                yield return null;
            }
        }
        
        private Dictionary<RectTransform, Coroutine> animations = new Dictionary<RectTransform, Coroutine>();
    }
}