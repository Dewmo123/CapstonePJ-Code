using System;
using Chipmunk.GameEvents;
using InGame.PlayerUI;
using UnityEngine;

namespace Code.UI.Core
{
    public class CursorManager : MonoBehaviour
    {
        private CursorLockMode _currentMode = CursorLockMode.None;

        private void Awake()
        {
            UIManager.Instance.OnStackChanged += HandleChangeStack;
            Cursor.lockState = _currentMode = CursorLockMode.Locked;
        }

        private void OnDestroy()
        {
            UIManager.Instance.OnStackChanged -= HandleChangeStack;
        }

        private void HandleChangeStack()
        {
            SetCursor();
        }
        
        private void SetCursor()
        {
            var newMode = UIManager.Instance.HasBlockingUI() ? CursorLockMode.None : CursorLockMode.Locked;
            
            if (_currentMode == newMode) return;
            Cursor.lockState = _currentMode = newMode;
            EventBus.Raise(new ChangeCursorEvent(newMode == CursorLockMode.Locked));
        }
    }
}