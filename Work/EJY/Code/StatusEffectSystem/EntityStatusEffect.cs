using System;
using Code.StatusEffectSystem.StatusEffects;
using System.Collections.Generic;
using System.Linq;
using Chipmunk.ComponentContainers;
using Chipmunk.GameEvents;
using Code.GameEvents;
using Scripts.Entities;
using UnityEngine;
namespace Code.StatusEffectSystem
{
    public struct StatusEffectInfo
    {
        public BuffSO KeySO;
        public StatusEffectEnum StatusEffect;
        public int Level;
        public float ApplyTime;
        public float Value;
        public bool IsPercent;
        public bool CanOverlap;
        public bool IsOverWrite;
    }
    public class EntityStatusEffect : MonoBehaviour, IContainerComponent
    {
        [SerializeField] private StatusEffectListSO statusEffectList;
        public event Action<AbstractStatusEffect> OnStatusEffectApplied;
        public event Action<AbstractStatusEffect> OnStatusEffectReleased;
        public ComponentContainer ComponentContainer { get; set; }
        private Dictionary<StatusEffectEnum, AbstractStatusEffect> _noneOverlapStatusEffects =
            new Dictionary<StatusEffectEnum, AbstractStatusEffect>();
        private Dictionary<BuffSO, List<AbstractStatusEffect>> _statusEffects = new();
        private Entity _target;
        private List<AbstractStatusEffect> _appliedStatusEffects = new List<AbstractStatusEffect>();
        public void OnInitialize(ComponentContainer componentContainer)
        {
            _target = componentContainer.Get<Entity>(true);
        }
        private void OnDestroy()
        {
            ClearStatusEffect();
        }
        private void Update()
        {
            for (int i = _appliedStatusEffects.Count - 1; i >= 0; i--)
            {
                var effect = _appliedStatusEffects[i];
                if (!effect.UpdateStatusEffect(_target))
                {
                    RemoveFromDictionaryAndFlag(effect);
                }
            }
        }
        private void RemoveFromDictionaryAndFlag(AbstractStatusEffect effect)
        {
            if (_noneOverlapStatusEffects.TryGetValue(effect.StatusEffectEnum, out var noneOverlapEffect))
            {
                if (noneOverlapEffect == effect)
                {
                    _noneOverlapStatusEffects.Remove(effect.StatusEffectEnum);
                }
            }
            if (effect.KeySO != null && _statusEffects.TryGetValue(effect.KeySO, out var list))
            {
                list.Remove(effect);
                if (list.Count == 0)
                    _statusEffects.Remove(effect.KeySO);
            }
            effect.ReleaseStatusEffect(_target);
            _appliedStatusEffects.Remove(effect);
            OnStatusEffectReleased?.Invoke(effect);
        }
        public AbstractStatusEffectDataSO GetStatusEffect(StatusEffectEnum statusEffect)
            => statusEffectList.GetStatusEffect(statusEffect);
        public AbstractStatusEffect AddStatusEffect(StatusEffectInfo statusEffectInfo)
        {
            if (_statusEffects.TryGetValue(statusEffectInfo.KeySO, out var list))
            {
                var appliedStatusEffect = list.FirstOrDefault(statusEffect =>
                    statusEffectInfo.StatusEffect == statusEffect.StatusEffectEnum);
                if (appliedStatusEffect != null)
                {
                    appliedStatusEffect.SetRemainingTime(Mathf.Max(statusEffectInfo.ApplyTime, appliedStatusEffect.CurrentTime));
                    return appliedStatusEffect;
                }
            }
            var data = GetStatusEffect(statusEffectInfo.StatusEffect);
            AbstractStatusEffect newStatusEffect = data.CreateStatusEffect(_target, statusEffectInfo);
            if (!data.canOverlap)
            {
                if (_noneOverlapStatusEffects.TryGetValue(statusEffectInfo.StatusEffect, out var oldEffect))
                {
                    if (statusEffectInfo.IsOverWrite || oldEffect.Level <= newStatusEffect.Level)
                    {
                        RemoveFromDictionaryAndFlag(oldEffect);
                    }
                    else
                    {
                        return null;
                    }
                }
                _noneOverlapStatusEffects[statusEffectInfo.StatusEffect] = newStatusEffect;
            }
            if (!_statusEffects.TryGetValue(statusEffectInfo.KeySO, out var newList))
            {
                newList = new List<AbstractStatusEffect>();
                _statusEffects.Add(statusEffectInfo.KeySO, newList);
            }
            newList.Add(newStatusEffect);
            newStatusEffect.ApplyStatusEffect(_target);
            _appliedStatusEffects.Add(newStatusEffect);
            OnStatusEffectApplied?.Invoke(newStatusEffect);
            return newStatusEffect;
        }
        public void RemoveStatusEffect(BuffSO buff)
        {
            if (_statusEffects.TryGetValue(buff, out List<AbstractStatusEffect> effectList))
            {
                for (int i = effectList.Count - 1; i >= 0; i--)
                {
                    var effect = effectList[i];
                    RemoveFromDictionaryAndFlag(effect);
                }
            }
        }
        public void ClearStatusEffect()
        {
            for (int i = _appliedStatusEffects.Count - 1; i >= 0; i--)
            {
                var effect = _appliedStatusEffects[i];
                effect.ReleaseStatusEffect(_target);
                OnStatusEffectReleased?.Invoke(effect);
            }
            _appliedStatusEffects.Clear();
            _noneOverlapStatusEffects.Clear();
            _statusEffects.Clear();
        }
        public bool IsStatusEffectExist(StatusEffectEnum statusEffect)
            => _noneOverlapStatusEffects.ContainsKey(statusEffect);
    }
}