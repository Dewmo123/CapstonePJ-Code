using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Tooltip
{
    public class TooltipMover : MonoBehaviour
    {
        [SerializeField] private Vector2 offset = new Vector2(15f, 15f);
        [SerializeField] private Canvas canvas;
        private RectTransform _rect;
        private RectTransform _canvasRect;

        private void Awake()
        {
            _rect = transform as RectTransform;
            _canvasRect = canvas.transform as RectTransform;

            _rect.anchorMin = new Vector2(0.5f, 0.5f);
            _rect.anchorMax = new Vector2(0.5f, 0.5f);
            _rect.pivot = new Vector2(0f, 1f);
        }

        private void LateUpdate()
        {
            SetPosition();
        }

        private void SetPosition()
        {
            if (_canvasRect == null) return;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);

            Vector2 localPoint;
            Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, Input.mousePosition, cam, out localPoint);

            float tw = _rect.rect.width;
            float th = _rect.rect.height;
            float cw = _canvasRect.rect.width;
            float ch = _canvasRect.rect.height;

            float posX = localPoint.x + offset.x;
            float posY = localPoint.y - offset.y;

            if (posX + tw > cw * 0.5f)
            {
                posX = localPoint.x - tw - offset.x;
            }

            if (posY - th < -ch * 0.5f)
            {
                posY = localPoint.y + th + offset.y;
            }

            posX = Mathf.Clamp(posX, -cw * 0.5f, cw * 0.5f - tw);
            posY = Mathf.Clamp(posY, -ch * 0.5f + th, ch * 0.5f);

            _rect.anchoredPosition = new Vector2(posX, posY);
        }
    }
}