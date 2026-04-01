using Scripts.Entities;
using UnityEngine;

namespace Code.StatusEffectSystem.StatusEffects
{
    public abstract class AbstractStatusEffect
    {
        public BuffSO KeySO { get; protected set; }
        public StatusEffectEnum StatusEffectEnum { get; protected set; }
        public Entity Target { get; protected set; }
        public int Level { get; protected set; }
        public bool CanOverlap { get; protected set; }
        public float ApplyTime { get; protected set; }
        public float CurrentTime { get; protected set; }
        public float Value { get; protected set; }
        public bool IsApplying { get; protected set; }

        public AbstractStatusEffect(Entity target, StatusEffectInfo statusEffectInfo)
        {
            Target = target;
            KeySO = statusEffectInfo.KeySO;
            StatusEffectEnum = statusEffectInfo.StatusEffect;
            Level = statusEffectInfo.Level;
            ApplyTime = statusEffectInfo.ApplyTime;
            CurrentTime = 0;
        }
        
        public void SetValue(float value) => Value = value;

        public virtual bool UpdateStatusEffect(Entity entity)
        {
            CurrentTime += Time.deltaTime;
    
            if(!IsApplying || CurrentTime >= ApplyTime)
                return false; // 더 이상 유지되지 않음 (제거 대상)
            return true; // 계속 유지됨
        }

        public virtual void ApplyStatusEffect(Entity entity)
        {
            CurrentTime = 0;
            IsApplying = true;  
        }
        
        public abstract void ReleaseStatusEffect(Entity entity);

        public void SetRemainingTime(float applyTime)
        {
            ApplyTime = applyTime;
            CurrentTime = 0;
        }
    }
}