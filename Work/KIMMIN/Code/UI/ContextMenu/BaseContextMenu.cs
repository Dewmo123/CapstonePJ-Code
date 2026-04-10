using System;
using System.Collections.Generic;
using Code.UI.Core;
using UnityEngine;

namespace Work.Code.UI.ContextMenu
{
    public abstract class BaseContextMenu : UIBase
    {
        [field: SerializeField] public ContextActionSO[] ContextActions { get; private set; }
        public override EUILayer Layer => EUILayer.ContextMenu;
        public Action OnAction;

        public abstract void ShowMenu(object data);
        public virtual void CloseMenu() => DisableUI(true);
    }
    
    public abstract class BaseContextMenu<T> : BaseContextMenu
    {
        [SerializeField] private Transform root;
        private readonly List<BaseContextAction<T>> _actions = new();
        private readonly Dictionary<Type, Queue<BaseContextAction<T>>> _pool = new();
        
        public sealed override void ShowMenu(object data)
        {
            EnableUI(true);
            ShowMenu((T)data);
        }

        protected virtual void ShowMenu(T data)
        {
            Clear();

            foreach (var actionSO in ContextActions)
            {
                var action = GetOrCreateAction(actionSO);
                if(!action.CanShow(data)) continue;
                InitAction(action, data);
            }
        }

        private void InitAction(BaseContextAction<T> action, T dataType)
        {
            action.Init(dataType);
            action.OnCallbackInvoked += HandleActionCalled;
            _actions.Add(action);
        }

        private void HandleActionCalled()
        {
            OnAction?.Invoke();
            CloseMenu();
        }

        private BaseContextAction<T> GetOrCreateAction(ContextActionSO action)
        {
            var prefab = action.contextAction as BaseContextAction<T>;
            if (_pool.TryGetValue(prefab.GetType(), out var queue) && queue.Count > 0)
                return queue.Dequeue();
            
            return Instantiate(prefab, root);
        }

        private void Clear()
        {
            foreach (var action in _actions)
            {
                var type = action.GetType();
                if (!_pool.ContainsKey(type))
                    _pool[type] = new();

                action.OnCallbackInvoked -= HandleActionCalled;
                action.DisableUI();
                _pool[type].Enqueue(action);
            }
            
            _actions.Clear();
        }
    }
}