using System;
using Code.UI.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Work.Code.Setting
{
    public class TitleOptionUI : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private RectTransform rect;

        private readonly Vector3 _highlightSize = new(275, 125);
        private readonly Vector3 _normalSize = new(250, 100);
        private UIEventHandler _uiEventHandler;
        private Tween _sizeTween;
        
        private void Awake()
        {
            _uiEventHandler = UIUtility.GetOrAddComponent<UIEventHandler>(gameObject);
            _uiEventHandler.BindUIEvent(gameObject, HandlePointerEnter, EUIEvent.PointerEnter);
            _uiEventHandler.BindUIEvent(gameObject, HandlePointerExit, EUIEvent.PointerExit);
        }

        private void OnDestroy()
        {
            UnbindPointerEvents();
        }

        private void HandlePointerEnter(PointerEventData evt)
        {
            _sizeTween?.Kill();
            _sizeTween = rect.DOSizeDelta(_highlightSize, 0.1f).SetUpdate(true);
        }
        
        private void HandlePointerExit(PointerEventData evt)
        {
            _sizeTween = rect.DOSizeDelta(_normalSize, 0.1f).SetUpdate(true);
        }

        public void UnbindPointerEvents()
        {
            _uiEventHandler.UnBindUIEvent(gameObject, HandlePointerEnter, EUIEvent.PointerEnter);
            _uiEventHandler.UnBindUIEvent(gameObject, HandlePointerExit, EUIEvent.PointerExit);
        }
    }
}