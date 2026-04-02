using System;
using System.Collections.Generic;
using Chipmunk.GameEvents;
using Code.UI.Core;
using Code.UI.Popup;
using UnityEngine;

namespace Code.UI.Controller
{
    public class PopupController : MonoBehaviour
    {
        [SerializeField] private List<BasePopup> popups;

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
            
            EventBus.Subscribe<BindPopupEvent>(HandleBindPopup);
            EventBus.Subscribe<UnBindPopupEvent>(HandleUnBindPopup);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<BindPopupEvent>(HandleBindPopup);
            EventBus.Unsubscribe<UnBindPopupEvent>(HandleUnBindPopup);
        }

        private void HandleUnBindPopup(UnBindPopupEvent evt)
        {
            
        }

        private void HandleBindPopup(BindPopupEvent evt)
        {
            BindPopup(evt.Popup);
        }

        public void BindPopup(IPopupable popup)
        {
            popup.OnClickHandler += HandleClickPopup;
        }

        public void UnbindPopup(IPopupable popup)
        {
            popup.OnClickHandler -= HandleClickPopup;
        }
        
        private void HandleClickPopup(Func<object> data, ICallbackData callback)
        {
            var type = data.Invoke();
            if (type == null) return;
            ShowPopup(type, callback);
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
        
        public void CloseTopPopup()
        {
            if (_popupStack.Count == 0) return;

            var popup = _popupStack.Pop();
            var type = popup.DataType;
            popup.ClosePopup();

            if (!_pool.ContainsKey(type))
                _pool[type] = new Stack<BasePopup>();

            _pool[type].Push(popup);
        }
    }
}