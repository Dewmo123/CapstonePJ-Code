using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Chipmunk.GameEvents;
using DG.Tweening;
using EasyTransition;
using UnityEngine;
using UnityEngine.UI;
using Work.Code.Core;
using Work.Code.GameEvents;

namespace Work.Code.Setting
{
    public class SelectCharacterUI : MonoBehaviour
    {
        [SerializeField] private Image blur;
        [SerializeField] private TransitionSettings transition;
        
        private CharacterTypeUI[] _characterTypes;

        private void Awake()
        {
            _characterTypes = GetComponentsInChildren<CharacterTypeUI>();
            foreach (var type in _characterTypes)
            {
                type.OnEnterHovering += HandleEnterHovering;
                type.OnExitHovering += HandleExitHovering;
                type.OnSelect += HandleSelect;
            }
            
            blur.DOFade(0f, 0f);
        }

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }

        private void UnSubscribeEvents()
        {
            foreach (var type in _characterTypes)
            {
                type.OnEnterHovering -= HandleEnterHovering;
                type.OnExitHovering -= HandleExitHovering;
                type.OnSelect -= HandleSelect;
            }
        }
        
        private void HandleSelect(CharacterTypeUI selectedType)
        {
            if (selectedType == null) return;
            EventBus.Raise(new SelectCharacterEvent(selectedType.Character));
            TransitionManager.Instance().Transition(SceneDefine.MAP_SCENE, transition, 3f);
            UnSubscribeEvents();
            
            foreach (var type in _characterTypes)
            {
                if (selectedType != type)
                    type.SetSize(0f, 0.5f, Ease.InBack);
                
                type.UnbindPointerEvents();
                type.ChardTween();
            }
        }

        private void HandleEnterHovering(CharacterTypeUI newType)
        {
            blur.DOKill();
            blur.DOFade(0.8f, 0.15f);
            newType.SetSortOrder(11);
            
            foreach (var type in _characterTypes)
            {
                if (type == newType)
                    type.SetSize(1.03f, 0.25f, Ease.OutCubic);
                else
                    type.SetSize(0.9f, 0.25f, Ease.OutCubic);
            }
        }
        
        private void HandleExitHovering()
        {
            blur.DOKill();
            blur.DOFade(0f, 0.15f);
            
            foreach (var type in _characterTypes)
            {
                type.SetSize(1f, 0.2f, Ease.OutCirc);
                type.SetSortOrder(1);
            }
        }
    }
}