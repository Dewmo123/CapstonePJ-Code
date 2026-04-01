using Chipmunk.ComponentContainers;
using Chipmunk.Library.Utility.GameEvents.Local;
using Scripts.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Entities
{
    public abstract class Entity : MonoBehaviour,IContainerComponent, IHitTransform
    {
        public delegate void OnHitDelegate(Entity dealer, IDamageable target);
        public delegate float OnDamageCalcDelegate(Entity dealer, Transform target);

        [SerializeField] private Transform hitBodyTrm;
        
        public bool IsDead { get; set; }
        public ComponentContainer ComponentContainer { get ; set ; }
        public Transform HitTransform => hitBodyTrm;

        public OnDamageCalcDelegate OnDamageCalc;
        public OnHitDelegate OnHit; // ���� �� ȿ�� (������ ���� ��)
        public UnityEvent OnHitEvent; // ������ ���� ��
        public UnityEvent OnDeadEvent;
        
        public LocalEventBus LocalEventBus { get; private set; }

        public virtual void OnInitialize(ComponentContainer componentContainer)
        {
            LocalEventBus = componentContainer.GetComponent<LocalEventBus>();
        }

        public void RotateToTarget(Vector3 targetPosition, bool isSmooth = false)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

            if (isSmooth)
            {
                const float smoothRotationSpeed = 15f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * smoothRotationSpeed);
            }
            else
            {
                transform.rotation = targetRotation;
            }
        }
        public virtual void Dead()
        {
            if (IsDead)
                return;
            OnDeadEvent?.Invoke();
        }

    }
}