using Assets.Work.AKH.Scripts.Entities.Vitals;
using Chipmunk.ComponentContainers;
using Code.StatusEffectSystem;
using Code.StatusEffectSystem.StatusEffects;
using Scripts.Combat.Datas;
using Scripts.Entities;
using UnityEngine;

namespace Work.Code.StatusEffects.Effects
{
    public class DotDealStatusEffect : AbstractStatusEffect
    {
        private float _tick = 0.5f;
        private float _tickTimer = 0f;
        private float _damagePerTick = 0f;
        private int _remainingTicks;
        private HealthCompo _targetHealth;
        
        public DotDealStatusEffect(Entity target, StatusEffectInfo statusEffectInfo) : base(target, statusEffectInfo)
        {
            _targetHealth = target.Get<HealthCompo>();
            Debug.Assert(_targetHealth != null, "Target has no health compo");
            _remainingTicks = Mathf.FloorToInt(ApplyTime / _tick);
            _damagePerTick = Value / _remainingTicks;
        }

        public override bool UpdateStatusEffect(Entity entity)
        {
            _tickTimer += Time.deltaTime;

            if (_tickTimer >= _tick && _remainingTicks > 0)
            {
                _tickTimer -= _tick;
                _remainingTicks--;
                DamageData damageData = new()
                {
                    damage = _damagePerTick,
                    damageType = DamageType.DOT,
                    defPierceLevel = 1
                };
                _targetHealth.TakeDamage(damageData);
            }

            return base.UpdateStatusEffect(entity);
        }

        public override void ReleaseStatusEffect(Entity entity)
        {

        }
    }
}