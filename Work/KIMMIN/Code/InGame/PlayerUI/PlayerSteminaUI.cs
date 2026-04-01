using System;
using Chipmunk.GameEvents;
using Chipmunk.Library.Utility.GameEvents.Local;
using DewmoLib.Dependencies;
using Scripts.Entities.Vitals;
using Scripts.Players;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.PlayerUI
{
    public class PlayerSteminaUI : MonoBehaviour
    {
        [SerializeField] private StaminaUI staminaUI;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Vector3 offset = new Vector3(1f, 0.5f, 0.25f);

        [Inject] private Player _player;
        private LocalEventBus _eventBus;
        private Camera _cam;
        private bool _isActive;
        
        private void Start()
        {
            _cam = Camera.main;
            canvas.worldCamera = _cam;
            
            _eventBus = _player.LocalEventBus;
            _eventBus.Subscribe<StaminaChangeEvent>(HandleStaminaChange);
        }

        private void OnDestroy()
        {
            _eventBus?.Unsubscribe<StaminaChangeEvent>(HandleStaminaChange);
        }

        private void HandleStaminaChange(StaminaChangeEvent evt)
        {
            if (Mathf.Approximately(evt.CurrentStamina, evt.MaxStamina))
            {
                staminaUI.DisableUI(true);
                _isActive = false;
                return;
            }

            if (!_isActive)
            {
                staminaUI.EnableUI(true);
                _isActive = true;
            }
            
            staminaUI.SetFill(evt.CurrentStamina / evt.MaxStamina);
        }

        private void LateUpdate()
        {
            if (_cam == null) return;
            gameObject.transform.forward = _cam.transform.forward;
            gameObject.transform.position = _player.transform.position + offset;
        }
    }
}