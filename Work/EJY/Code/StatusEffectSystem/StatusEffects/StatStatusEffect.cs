using Chipmunk.Modules.StatSystem;
using Scripts.Entities;
using UnityEngine;

namespace Code.StatusEffectSystem.StatusEffects
{
    public class StatStatusEffect : AbstractStatusEffect
    {
        protected StatOverrideBehavior _targetStat;
        protected StatSO _targetStatSO;

        public StatStatusEffect(Entity target, StatusEffectInfo statusEffectInfo, StatSO statSO) : base(target, statusEffectInfo)
        {
            _targetStat = target.ComponentContainer.Get<StatOverrideBehavior>();
            _targetStatSO = statSO;

            Debug.Assert(_targetStat != null, "Target has not stat component");
        }
        
        public override void ApplyStatusEffect(Entity entity)
        {
            if (_targetStatSO == null)
            {
                Debug.Log("Target stat is null");
                IsApplying = false;
                return;
            }
            
            base.ApplyStatusEffect(entity);
            _targetStat.GetStat(_targetStatSO).AddValueModifier(this, Value);
        }

        public override void ReleaseStatusEffect(Entity entity)
        {
            if (_targetStatSO == null)
            {
                Debug.Log("Target stat is null");
                IsApplying = false;
                return;
            }
            
            _targetStat.GetStat(_targetStatSO).RemoveModifier(this);
        }
    }
}