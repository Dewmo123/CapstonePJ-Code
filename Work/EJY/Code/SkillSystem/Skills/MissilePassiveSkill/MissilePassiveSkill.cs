using System.Collections.Generic;
using DewmoLib.Dependencies;
using DewmoLib.ObjectPool.RunTime;
using Scripts.Combat;
using Scripts.Entities;
using Scripts.SkillSystem;
using UnityEngine;
using UnityEngine.Splines;

namespace Code.SkillSystem.Skills.MissilePassiveSkill
{
    public class MissilePassiveSkill : PassiveSkill
    {
        [SerializeField] private Transform firePosTrm;
        [SerializeField] private int hitCntToFireMissile = 5;
        [SerializeField] private int multiShotMissile = 3;
        [SerializeField] private PoolItemSO missilePoolItem;
        [SerializeField] private bool isDmgRangIncrease;
        [SerializeField] private bool isInduction;
        [SerializeField] private float additionalDmgRange = 2.5f;
        [SerializeField] private float randomAdditionalX = 2.5f;
        [SerializeField] private float randomAdditionalZ = 2.5f;

        [Inject] private PoolManagerMono _poolManager;
        private int _currentHitCnt = 0;

        public override void EnableSkill()
        {
            base.EnableSkill();
            _owner.OnHit += HandleOnHit;
        }

        public override void DisableSkill()
        {
            _owner.OnHit -= HandleOnHit;
            base.DisableSkill();
        }
        
        private void SetDmgRangeIncrease() => isDmgRangIncrease = true;
        private void SetInduction() => isInduction = true;

        private void HandleOnHit(Entity dealer, IDamageable target)
        {
            if (target is not MonoBehaviour targetMono)
            {
                Debug.Log("target is not MonoBehaviour");
                return;
            }

            _currentHitCnt++;

            if (_currentHitCnt >= hitCntToFireMissile)
            {
                Transform targetRootTrm = targetMono.transform.root;
                IHitTransform hitTransform = targetRootTrm.gameObject.GetComponent<IHitTransform>();

                if(hitTransform == null) return;
                
                for (int i = 0; i < multiShotMissile; ++i)
                {
                    var missile = _poolManager.Pop<Missile>(missilePoolItem);
                    missile.InitMissile(_owner, hitTransform.HitTransform, firePosTrm.position, isInduction);
                    if (isDmgRangIncrease) missile.SetDmgRange(additionalDmgRange);
                    var path = GenerateMissilePath(firePosTrm.position, hitTransform.HitTransform.position);
                    missile.SetPathToTarget(path);
                }
                
                _currentHitCnt = 0;
            }
        }

        private Spline GenerateMissilePath(Vector3 start, Vector3 end)
        {
            Spline pathSpline = new Spline();
            pathSpline.Add(new BezierKnot(start));

            Vector3 ownerUp = start + Vector3.up * 2;
            
            float x = Random.Range(-randomAdditionalX, randomAdditionalX);
            float z = Random.Range(-randomAdditionalZ, randomAdditionalZ);

            ownerUp.x += x;
            ownerUp.z += z;
            pathSpline.Add(new BezierKnot(ownerUp));
            
            pathSpline.Add(new BezierKnot(end));

            return pathSpline;
        }
    }
}