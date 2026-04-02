using System;
using Chipmunk.GameEvents;
using Code.UI.Core;
using Code.UI.Popup;
using UnityEngine;
using Work.Code.GameEvents;

namespace Work.Code.UI.Core.Interaction
{
    [RequireComponent(typeof(UIEventHandler))]
    public class InteractableUI : UIBase
    {
        protected UIEventHandler _eventHandler;

        protected override void Awake()
        {
            base.Awake();
            _eventHandler = GetComponent<UIEventHandler>();
        }

        protected override void OnDestroy()
        {
            _eventHandler?.ClearAll();
            ClearInteractEvents();
        }

        protected void BindTooltip(GameObject go, Func<object> data, float duration = 0f)
        {
            EventBus.Raise(new BindTooltipEvent(go, data, duration));
        }

        protected void UnbindTooltip(GameObject go)
        {
            EventBus.Raise(new UnBindTooltipEvent(go));
        }
        
        protected void BindPopup(IPopupable popup)
        {
            EventBus.Raise(new BindPopupEvent(popup));
        }
        
        protected void UnBindPopup(IPopupable popup)
        {
            EventBus.Raise(new UnBindPopupEvent(popup));
        }

        protected virtual void ClearInteractEvents() { }
    }
}