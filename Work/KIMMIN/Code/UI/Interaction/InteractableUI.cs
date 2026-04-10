using System;
using Chipmunk.GameEvents;
using Code.UI.Core;
using Code.UI.Popup;
using UnityEngine;
using Work.Code.GameEvents;
using Work.Code.UI.ContextMenu;

namespace Work.Code.UI.Core.Interaction
{
    [RequireComponent(typeof(UIEventHandler))]
    public class InteractableUI : UIBase
    {
        public UIEventHandler EventHandler { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            EventHandler = GetComponent<UIEventHandler>();
        }

        protected override void OnDestroy()
        {
            ClearInteractEvents();
        }
        
        protected void BindTooltip(Func<object> data, float duration = 0f)
        {
            EventBus.Raise(new BindTooltipEvent(this, data, duration));
        }

        protected void UnbindTooltip()
        {
            EventBus.Raise(new UnBindTooltipEvent(this));
        }
        
        protected void BindContextMneu(ContextMenuSO menu, Func<object> data)
        {
            EventBus.Raise(new BindContextMenuEvent(this, menu, data));
        }

        protected void UnBindContextMneu()
        {
            EventBus.Raise(new UnBindContextMenuEvent(this));
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