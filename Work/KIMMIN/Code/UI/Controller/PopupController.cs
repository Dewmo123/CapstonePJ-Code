using System;
using System.Collections.Generic;
using Code.UI.Core;
using Code.UI.Core.Interaction;
using Code.UI.Popup;
using UnityEngine;

namespace Code.UI.Controller
{
    public class PopupController : MonoBehaviour
    {
        [SerializeField] private List<BasePopup> popups;
        [SerializeField] private Transform layoutRoot;

        private Dictionary<Type, BasePopup> _popupMap = new();
        private Dictionary<Type, Stack<BasePopup>> _pool = new();
        private Stack<BasePopup> _popupStack = new();
        private Transform _root;

        private void Awake()
        {
            _root = UIManager.Instance.GetLayer(EUILayer.Popup);

            foreach (var popup in popups)
            {
                if (popup == null) continue;
                _popupMap.TryAdd(popup.DataType, popup);
            }
        }

        public void BindPopup<TData>(IPopupUI popup, Func<TData> data)
        {
            popup.OnClickHandler += HandleClickPopup;
        }

        public void UnbindPopup(IPopupUI popup)
        {
            popup.OnClickHandler -= HandleClickPopup;
        }
        
        public void ShowPopup(object data, ICallbackData callback = null)
        {
            var type = data.GetType();

            if (!_popupMap.TryGetValue(type, out var prefab))
            {
                Debug.LogWarning($"Popup not found for type: {type}");
                return;
            }

            BasePopup popup;
            if (_pool.TryGetValue(type, out var stack) && stack.Count > 0)
                popup = stack.Pop();
            else
                popup = Instantiate(prefab, _root);

            popup.ShowPopup(data, callback);
            _popupStack.Push(popup);
        }

        private void HandleClickPopup(IPopupUI parent, Func<object> data)
        {
            
        }

        private void ShowPopup(IPopupUI popup, Func<object> data)
        {
            
        }

        private void OnDestroy()
        {
            
        }
    }
}