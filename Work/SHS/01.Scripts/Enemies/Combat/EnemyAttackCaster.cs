using Chipmunk.ComponentContainers;
using Code.SHS.Animations;
using Scripts.Combat;
using Scripts.Combat.Datas;
using UnityEngine;

namespace Code.SHS.Entities.Enemies.Combat
{
    public class EnemyAttackCaster : MonoBehaviour, IExcludeContainerComponent
    {
        [SerializeField] private TriggerTypeSO triggerType;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private float damageAmount;

        public ComponentContainer ComponentContainer { get; set; }

        public Enemy Owner { get; private set; }

        public void OnInitialize(ComponentContainer componentContainer)
        {
            AnimatorTrigger animatorTrigger = componentContainer.GetComponent<AnimatorTrigger>();
            animatorTrigger.OnAttackTrigger += HandleAttackTrigger;
            Owner = componentContainer.Get<Enemy>(true);
        }

        private void HandleAttackTrigger(TriggerTypeSO obj)
        {
            if (obj != triggerType) return;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.5f, targetLayer);
            foreach (var hitCollider in hitColliders)
            {
                ComponentContainer target = hitCollider.GetComponent<ComponentContainer>();
                if (target != null && target.TryGetSubclassComponent(out IDamageable damageable))
                {
                    DamageData damageData = new DamageData
                    {
                        damage = damageAmount,
                        damageType = DamageType.MELEE
                    };

                    DamageContext context = new DamageContext
                    {
                        DamageData = damageData,
                        HitPoint = transform.position,
                        HitNormal = Vector3.back,
                        Source = Owner.gameObject,
                        Attacker = Owner
                    };
                        
                    damageable.ApplyDamage(context);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1.5f);
        }
    }
}