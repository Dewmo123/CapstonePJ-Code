using System;
using DG.Tweening;
using UnityEngine;

namespace Code.UI.Core
{
    public class UIBase : MonoBehaviour
    {
        public CanvasGroup CanvasGroup { get; protected set; }
        public bool IsActive { get; protected set; }
        public virtual EUILayer Layer => EUILayer.None;
        public event Action<UIBase, bool> OnToggleUI;

        protected virtual void Awake()
        {
            CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            UIManager.Instance?.RegisterUI(this);
        }

        public virtual void EnableUI(bool isFade = false)
        {
            if (IsActive) return;
            IsActive = true;
            OnToggleUI?.Invoke(this, isFade);
        }

        public virtual void DisableUI(bool isFade = false)
        {
            if (!IsActive) return;
            IsActive = false;
            OnToggleUI?.Invoke(this, isFade);
        }

        public virtual void ToggleUI(bool isFade = false)
        {
            if (IsActive)
                DisableUI(isFade);
            else
                EnableUI(isFade);
        }
        
        protected virtual void OnDestroy()
        {
            if (UIManager.HasInstance)
            {
                UIManager.Instance?.UnRegisterUI(this);
            }
        }
        
        [ContextMenu("Show UI")]
        public void ShowUI()
        {
            CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }

        [ContextMenu("Hide UI")]
        public void HideUI()
        {
            CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }
    }
}