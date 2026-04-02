using System;
using Chipmunk.GameEvents;
using UnityEngine;

namespace Code.UI.Popup
{
    public struct BindPopupEvent : IEvent
    {
        public IPopupable Popup { get; }
        
        public BindPopupEvent(IPopupable popup)
        {
            Popup = popup;
        }
    }
    
    public struct UnBindPopupEvent : IEvent
    {
        public IPopupable Popup { get; }
        public Func<object> Data { get; }

        public UnBindPopupEvent(IPopupable popup, Func<object> dataCallback = null)
        {
            Popup = popup;
            Data = dataCallback;
        }
    }
}