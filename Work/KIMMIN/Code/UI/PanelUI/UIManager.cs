using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Chipmunk.GameEvents;
using Code.GameEvents;
using DG.Tweening;
using UnityEngine;
using Work.Code.Core;
using Work.Code.GameEvents;

namespace Code.UI.Core
{
    public enum EUILayer
    {
        HUD,
        Panel,
        Popup,
        Tooltip,
        None
    }
    
    public class UIManager : MonoSingleton<UIManager>
    { 
        [SerializeField] private PlayerInputSO playerInput;
        [SerializeField] private SerializedDictionary<EUILayer, Transform> layerMap;

        private readonly Stack<UIBase> _uiStack = new();
        private readonly HashSet<UIBase> _registeredUIs = new();
        
        public event Action OnStackChanged;

        private void Awake()
        {
            playerInput.OnToggleUIPressed += HandlePressEsc;
        }

        private void OnDestroy()
        {
            foreach (var ui in _registeredUIs)
            {
                ui.OnToggleUI -= HandleChnageUIState;
            }
            
            playerInput.OnToggleUIPressed -= HandlePressEsc;
        }
        
        public void RegisterUI(UIBase ui)
        {
            if (!_registeredUIs.Add(ui)) return;
            ui.OnToggleUI += HandleChnageUIState;
        }

        public void UnRegisterUI(UIBase ui)
        {
            if (!_registeredUIs.Contains(ui)) return;
            _registeredUIs.Remove(ui);
            ui.OnToggleUI -= HandleChnageUIState;
        }
        
        private void HandleChnageUIState(UIBase ui, bool isFade)
        {
            HandleStack(ui, ui.IsActive);
            HandleToggle(ui, ui.IsActive, isFade);
        }

        private void HandleStack(UIBase ui, bool isActive)
        {
            if (!CanStack(ui)) return;

            if (isActive)
                PushUI(ui);
            else
                RemoveUI(ui);
            
            OnStackChanged?.Invoke();
            playerInput.SetPlayerInput(_uiStack.Count == 0);
        }

        private bool CanStack(UIBase ui)
        {
            return ui.Layer == EUILayer.Panel || ui.Layer == EUILayer.Popup;
        }

        public Transform GetLayer(EUILayer layer)
        {
            return layerMap.GetValueOrDefault(layer);
        }

        private void HandlePressEsc()
        {
            if (_uiStack.Count == 0)
            {
                EventBus.Raise(new PressESCEvent());
                return;
            }
            
            EventBus.Raise(new PlayerUIEvent(false));
            PopUI();
        }

        public void PushUI(UIBase ui)
        {
            if (_uiStack.Contains(ui)) return;

            if (ui.Layer == EUILayer.Panel)
                ClearStack();

            _uiStack.Push(ui);
        }

        private void ClearStack()
        {
            while (_uiStack.Count > 0)
            {
                var top = _uiStack.Pop();
                top.DisableUI();
            }
        }
        
        public UIBase PopUI()
        {
            if (_uiStack.Count == 0) return null;

            var top = _uiStack.Pop();
            top.DisableUI();
            return top;
        }
        
        public void RemoveUI(UIBase target)
        {
            if (!_uiStack.Contains(target)) return;

            Stack<UIBase> temp = new Stack<UIBase>();

            while (_uiStack.Peek() != target)
                temp.Push(_uiStack.Pop());

            _uiStack.Pop().DisableUI();

            while (temp.Count > 0)
                _uiStack.Push(temp.Pop());
        }
        
        public bool HasBlockingUI()
        {
            foreach (var ui in _uiStack)
            {
                if (ui.Layer == EUILayer.Panel)
                    return true;
            }
            return false;
        }
        
        private void HandleToggle(UIBase ui, bool isActive, bool useFade)
        {
            var cg = ui.CanvasGroup;
            cg.DOKill();

            if (useFade)
            {
                if (isActive) 
                {
                    cg.alpha = 0;
                    cg.interactable = true;
                    cg.blocksRaycasts = true;
                    cg.DOFade(1, 0.1f).SetUpdate(true);
                }
                else 
                {
                    cg.DOFade(0, 0.1f).OnComplete(() => 
                    { 
                        cg.interactable = false;
                        cg.blocksRaycasts = false; 
                    }).SetUpdate(true);
                }
            }
            else
            {
                cg.alpha = isActive ? 1 : 0;
                cg.interactable = isActive;
                cg.blocksRaycasts = isActive;
            }
        }
    }
}