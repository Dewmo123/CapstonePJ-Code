using System;
using Code.UI.Core;
using Code.UI.Popup;

namespace Work.Code.UI.ContextMenu
{
    public abstract class BaseContextMenu : UIBase
    {
        public override EUILayer Layer => EUILayer.Popup;
        public abstract Type DataType { get; }
        public abstract void ShowPopup(object data, Action callback);
        public virtual void ClosePopup() => DisableUI(true);

        public void SetPriority(int priority)
        {
            transform.SetSiblingIndex(priority);
        }
    }
    
    public abstract class BaseContextMenu<TData> : BaseContextMenu
    {
    }
}