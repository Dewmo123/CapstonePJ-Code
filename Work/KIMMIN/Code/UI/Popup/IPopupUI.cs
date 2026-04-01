using System;
using Code.UI.Core.Interaction;

namespace Code.UI.Popup
{
    public interface IPopupUI : IClickable
    {
        public event Action<IPopupUI, Func<object>> OnClickHandler;
    }
}