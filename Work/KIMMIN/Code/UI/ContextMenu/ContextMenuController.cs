using System;
using System.Collections.Generic;
using Chipmunk.GameEvents;
using Code.UI.Core;
using UnityEngine;
using UnityEngine.UI;
using Work.Code.UI.Core.Interaction;

namespace Work.Code.UI.ContextMenu
{
    public class ContextMenuData
    {
        public ContextMenuSO MenuSO;
        public Func<object> Data;
    }

    [DefaultExecutionOrder(-10)]
    public class ContextMenuController : MonoBehaviour
    {
        [SerializeField] private List<ContextMenuSO> menus;
        [SerializeField] private RectTransform menuParent;
        [SerializeField] private RectTransform menuRoot;
        [SerializeField] private ContextMenuPanel panel;
        
        private readonly Dictionary<InteractableUI, ContextMenuData> _contextMenus = new();
        private readonly Dictionary<ContextMenuSO, BaseContextMenu> _instances = new();
        private BaseContextMenu _currentMenu;

        private void Awake()
        {
            MappingMenus();
            EventBus.Subscribe<BindContextMenuEvent>(HandleBindMenu);
            EventBus.Subscribe<UnBindContextMenuEvent>(HandleUnBindMenu);
            panel.PanelButton.onClick.AddListener(HandleClickPanel);
        }

        private void HandleClickPanel()
        {
            HideCurrentMenu();
        }

        private void MappingMenus()
        {
            foreach (var menu in menus)
            {
                if (menu == null) continue;
                var newMenu = Instantiate(menu.menu, menuRoot);
                newMenu.DisableUI();
                _instances.TryAdd(menu, newMenu);
            }
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<BindContextMenuEvent>(HandleBindMenu);
            EventBus.Unsubscribe<UnBindContextMenuEvent>(HandleUnBindMenu);
            panel.PanelButton.onClick.RemoveListener(HandleClickPanel);
        }

        private void HandleBindMenu(BindContextMenuEvent evt)
        {
            BindContextMenu(evt.Owner, evt.ContextMenu, evt.Data);
        }

        private void HandleUnBindMenu(UnBindContextMenuEvent evt)
        { 
            HideCurrentMenu();
            UnbindContextMenu(evt.Owner);
        }

        public void BindContextMenu<T>(InteractableUI owner, ContextMenuSO menu, Func<T> data)
        {
            _contextMenus[owner] = new ContextMenuData { MenuSO = menu, Data = () => data() };
            owner.OnToggleUI += HandleToggleUI;
            
            owner.EventHandler.BindUIEvent(owner, _ =>
            {
                ShowContextMenu(owner);
            }, EUIEvent.RightClick);
        }

        private void HandleToggleUI(UIBase ui, bool isActive)
        {
            if(!isActive) HideCurrentMenu();    
        }

        public void UnbindContextMenu(InteractableUI owner)
        {
            if (_contextMenus.ContainsKey(owner))
                _contextMenus.Remove(owner);

            owner.OnToggleUI -= HandleToggleUI;
            owner.EventHandler.ClearUIEvent(owner, EUIEvent.RightClick);
        }

        private void ShowContextMenu(InteractableUI owner)
        {
            if (!_contextMenus.TryGetValue(owner, out var menuData)) return;

            var data = menuData.Data?.Invoke();
            if (data == null) return;

            HideCurrentMenu();
            
            if (_instances.TryGetValue(menuData.MenuSO, out BaseContextMenu menu))
            {
                _currentMenu = menu;
                _currentMenu.ShowMenu(data);
                _currentMenu.OnAction += HideCurrentMenu;
                SetPosition(owner.Rect);
            }
            
            panel.EnableUI();
        }

        public void HideCurrentMenu()
        {
            if (_currentMenu == null) return;

            _currentMenu.OnAction -= HideCurrentMenu;
            _currentMenu.CloseMenu();
            _currentMenu = null;
            panel.DisableUI();
        }

        private void SetPosition(RectTransform rect)
        {
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            Vector3 topRight = corners[2];

            RectTransformUtility.ScreenPointToLocalPointInRectangle(menuRoot, 
                RectTransformUtility.WorldToScreenPoint(null, topRight), 
                null, out Vector2 localPoint);

            _currentMenu.Rect.anchoredPosition = new Vector2(localPoint.x, localPoint.y);
        }
    }
}