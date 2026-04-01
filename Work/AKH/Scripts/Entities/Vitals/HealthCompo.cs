using Chipmunk.ComponentContainers;
using Chipmunk.GameEvents;
using Chipmunk.Modules.StatSystem;
using Scripts.Combat;
using Scripts.Combat.Datas;
using Scripts.Entities;
using Scripts.Entities.Vitals;
using SHS.Scripts.Combats.Events;
using System;
using Chipmunk.Library.Utility.GameEvents.Local;
using UnityEngine;
using Work.Code.GameEvents;

namespace Assets.Work.AKH.Scripts.Entities.Vitals
{
    public class HealthCompo : VitalManageCompo<HealthChangeEvent>, IDamageable
    {
        [SerializeField] private StatSO defStat, damageDemodifyStat;
        private ShieldCompo _shieldCompo;
        public event Action<float> OnTakeDamage;
        public event Action<DamageContext> OnHit;
        public event Func<DamageContext, bool> OnBeforeHit;

        public override void OnInitialize(ComponentContainer componentContainer)
        {
            base.OnInitialize(componentContainer);
            _shieldCompo = componentContainer.Get<ShieldCompo>();
        }

        public override void AfterInitialize()
        {
            base.AfterInitialize();
            defStat = _statCompo.GetStat(defStat);
            damageDemodifyStat = _statCompo.GetStat(damageDemodifyStat);
        }

        public void ApplyDamage(DamageData damageData, Vector3 hitPoint, Vector3 hitNormal, Entity dealer)
        {
            TakeDamage(damageData);
            _localEventBus.Raise(new DamagedEvent(_entity, damageData, hitPoint, hitNormal, dealer));
        }

        public void ApplyDamage(DamageContext context)
        {
            if (OnBeforeHit != null)
            {
                foreach (var func in OnBeforeHit.GetInvocationList())
                {
                    var del = (Func<DamageContext, bool>)func;
                    if (del(context))
                        return;
                }
            }

            TakeDamage(context.DamageData);
            _localEventBus.Raise(new DamagedEvent(_entity,
                context.DamageData, context.HitPoint,
                context.HitNormal, context.Attacker));

            if (Mathf.Approximately(CurrentValue, 0))
            {
                _entity.Dead();
                _localEventBus.Raise(new EntityDeadEvent(context.HitPoint,context.HitNormal));
            }

            OnHit?.Invoke(context);
        }

        [ContextMenu("Test Take Damage")]
        public void TestTakeDamage() => TakeDamage(new DamageData
            { damage = 10, damageType = DamageType.DOT, defPierceLevel = 1 });

        public void TakeDamage(DamageData damageData)
        {
            float finalDefModify = 2 / (Mathf.Max(defStat.Value - damageData.defPierceLevel, 0) + 2);
            float finalDamage = damageData.damage * finalDefModify * damageDemodifyStat.Value;

            if (_shieldCompo != null)
            {
                finalDamage = _shieldCompo.DamageDecreaseByShield(finalDamage);
            }

            CurrentValue -= finalDamage;
            OnTakeDamage?.Invoke(finalDamage);
            _entity.OnHitEvent?.Invoke();
            EventBus.Raise(new DamageTextEvent(finalDamage, transform.position));
        }

        public void ApplyDamage(DamageData damageData, Entity dealer)
        {
            Vector3 hitPoint = transform.position;
            Vector3 hitNormal = (dealer.transform.position - transform.position).normalized;

            if (TryGetComponent<Collider>(out var col))
            {
                hitPoint = col.ClosestPoint(dealer.transform.position);
            }

            DamageContext context = new DamageContext
            {
                DamageData = damageData,
                HitPoint = hitPoint,
                HitNormal = hitNormal,
                Source = dealer.gameObject,
                Attacker = dealer
            };

            ApplyDamage(context);
        }
    }
}