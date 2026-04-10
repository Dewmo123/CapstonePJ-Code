using System;
using System.Threading;
using Code.StatusEffectSystem;
using Cysharp.Threading.Tasks;
using DewmoLib.Dependencies;
using DewmoLib.ObjectPool.RunTime;
using Scripts.Entities;
using Scripts.SkillSystem;
using UnityEngine;
using Exception = System.Exception;
using Random = UnityEngine.Random;

namespace Code.SkillSystem.Skills.TrackingBlade
{
    public class TrackingBladeSkill : ActiveSkill
    {
        [SerializeField] private PoolItemSO trackingBladeItemSO;
        [SerializeField] private LayerMask whatIsEnemy;
        [SerializeField] private Transform firePosTrm;
        [SerializeField] private float delayToFire = 1.5f;
        [SerializeField] private float detectRange = 8f;
        [SerializeField] private int trackingBladeCnt = 1;
        [SerializeField] private int additionalTrackingBladeCnt = 1;
        
        [Inject] private PoolManagerMono _poolManager;
        private Entity _target;

        public override bool CanUseSkill()
        {
            _target = SearchTarget();
            return base.CanUseSkill() && _target != null;
        }

        public override void StartAndUseSkill()
        {
            if (_target != null)
            {
                StartLockOn();
            }
        }

        private async void StartLockOn()
        {
            bool success = await Charge();

            if (success)
            {
                for (int i = 0; i < trackingBladeCnt; i++)
                {
                    Vector3 firePos = firePosTrm.position;

                    float x = Random.Range(-0.5f, 0.5f);
                    float z = Random.Range(-0.5f, 0.5f);
                   
                    firePos.x += x;
                    firePos.z += z;

                    TrackingBlade tb = _poolManager.Pop<TrackingBlade>(trackingBladeItemSO);
                    
                    Quaternion rotate = Quaternion.Euler(0, Random.Range(-80f, 80f), 0);
                    tb.Initialize(_target ,firePos, rotate * firePosTrm.forward);
                }
            }
            else
            {
                float cooldownAmount = cooldown * 0.4f;
                // 현재 쿨다운에서 감소
            }
        }

        private async UniTask<bool> Charge()
        {
            var thisObjectDestroyCts = destroyCancellationToken;
            var targetDestroyCts = _target.destroyCancellationToken;
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(thisObjectDestroyCts, targetDestroyCts);

            try
            {
                await UniTask.WaitForSeconds(delayToFire, cancellationToken:linkedCts.Token);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private Entity SearchTarget()
        {
             var results = Physics.OverlapSphere(transform.position, detectRange, whatIsEnemy);

             foreach (var col in results)
             {
                 if (col.TryGetComponent(out Entity target))
                 {
                     return target;
                 }
             }
             
             return null;
        }
    }
}