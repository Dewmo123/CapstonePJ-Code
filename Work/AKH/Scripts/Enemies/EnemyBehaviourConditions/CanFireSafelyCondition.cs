using Chipmunk.ComponentContainers;
using Code.SHS.Entities.Enemies;
using Scripts.Combat;
using Scripts.Combat.Datas;
using Scripts.Enemies.EnemyBehaviourConditions;
using UnityEngine;

namespace Enemies.EnemyBehaviourConditions
{
    public class CanFireSafelyCondition : EnemyBehaviourCondition
    {
        [SerializeField] private LayerMask dontShootLayer;
        [SerializeField] private Vector3 halfBox;
        private AttackCompo _attackCompo;
        public override void Init(Enemy enemy)
        {
            base.Init(enemy);
            _attackCompo = enemy.Get<AttackCompo>();
        }
        public override bool Condition()
        {
            GunItem gunItem = _attackCompo.GetCurrentWeapon<GunItem>();
            if (gunItem == null)
                return true;
            Transform fireTrm = gunItem.GunObj.FireTrm;
            Vector3 fireDirection = gunItem.GunObj.FireDirection.normalized;
            Vector3 targetPos = _enemy.TargetProvider.Target.transform.position;
            float attackRange = Vector3.Distance(targetPos, fireTrm.position);
            return !Physics.BoxCast(fireTrm.position, halfBox, fireDirection, Quaternion.identity, attackRange, dontShootLayer);
        }
#if UNITY_EDITOR
        public override void DrawGizmos(Transform trm)
        {
            base.DrawGizmos(trm);
            Gizmos.color = Color.red;
            if (_attackCompo == null)
                Gizmos.DrawWireCube(trm.position, halfBox);
            else
            {
                GunItem gunItem = _attackCompo.GetCurrentWeapon<GunItem>();
                if (gunItem != null)
                    Gizmos.DrawWireCube(gunItem.GunObj.FirePosition, halfBox);

            }
        }
#endif
    }
}
