using System;
using Chipmunk.ComponentContainers;
using Chipmunk.Modules.StatSystem;
using Code.StatusEffectSystem;
using Cysharp.Threading.Tasks;
using Scripts.Combat;
using Scripts.Entities;
using Scripts.SkillSystem;
using UnityEngine;

namespace Code.SkillSystem.Skills.FireRate
{
    public class FireRateSkill : ActiveSkill
    {
        [SerializeField] private BuffSO fireRateBuffSO;
        [SerializeField] private StatSO fireRateStatSO;
        [SerializeField] private StatusEffectCreateData bulletReduceRateData;
        [SerializeField] private bool isOnHitAddFireRate;
        [SerializeField] private float onHitFireRateAmount = 0.025f, maxFireRate = 0.5f;
        
        private EntityStatusEffect _entityStatusEffect;
        private StatOverrideBehavior _stat;
        private float _totalFireRate;
        
        public override void Init(ComponentContainer container)
        {
            base.Init(container);
            _entityStatusEffect = container.Get<EntityStatusEffect>();
            _stat = container.Get<StatOverrideBehavior>();
        }

        [ContextMenu("Bullet Reduce Rate")]
        private void AddBulletReduceRateDecrease()
        {
            fireRateBuffSO.statusEffectCreateData.Add(bulletReduceRateData);
        }
        
        public override async void StartAndUseSkill()
        {
            foreach (var info in fireRateBuffSO.GetStatusEffectInfo())
            {
                _entityStatusEffect.AddStatusEffect(info);
            }

            if (isOnHitAddFireRate)
            {
                var cts = this.GetCancellationTokenOnDestroy();   
                
                _owner.OnHit += OnHitAddFireRate;
                
                try 
                {
                    await UniTask.WaitForSeconds(fireRateBuffSO.applyTime, cancellationToken: cts);
                }
                catch (Exception e) 
                {
                    Debug.Log(e);
                    return;
                }
                finally 
                {
                    _owner.OnHit -= OnHitAddFireRate;
                    _totalFireRate = 0;
                }
            }
        }

        private void OnHitAddFireRate(Entity dealer, IDamageable target)
        {
            _totalFireRate += onHitFireRateAmount;
            _totalFireRate = Mathf.Min(_totalFireRate, maxFireRate);
            
            var targetStat = _stat.GetStat(fireRateStatSO);
            targetStat.RemoveModifier(this);
            targetStat.AddValueModifier(this,-_totalFireRate);
        }
    }
}