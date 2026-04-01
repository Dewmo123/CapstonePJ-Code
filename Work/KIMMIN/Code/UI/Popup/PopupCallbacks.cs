using System;

namespace Code.UI.Popup
{
    public interface ICallbackData { }
    
    public struct EmptyCallback : ICallbackData { }

    public struct ConfirmCallback : ICallbackData
    {
        public Action OnConfirm;
    }
    
    public struct ChoiceCallback : ICallbackData
    {
        public Action OnConfirm;
    }
}