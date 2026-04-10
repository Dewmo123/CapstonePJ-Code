using System;
using Chipmunk.ComponentContainers;
using Chipmunk.GameEvents;
using Code.GameEvents;
using Code.StatusEffectSystem;
using DewmoLib.ObjectPool.RunTime;
using Scripts.Combat;
using Scripts.Entities;
using UnityEngine;

namespace Code.SkillSystem.Skills.TrackingBlade
{
    public class TrackingBlade : MonoBehaviour, IPoolable
    {
        [SerializeField] private PoolItemSO trackingBladeItemSO;
        [SerializeField] private PoolItemSO trackingBladeHitItemSO;
        [SerializeField] private BuffSO bleedingBuff;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float rotationSpeed = 120f;
        [SerializeField] private float delayToRotate = 0.5f;

        public PoolItemSO PoolItem => trackingBladeItemSO;
        public GameObject GameObject => gameObject;

        private Rigidbody _rigidbody;
        private Pool _myPool;
        private Entity _target;
        private float _currentTime;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }

        public void Initialize(Entity target ,Vector3 position, Vector3 direction)
        {
            trailRenderer?.Clear();
            
            _target = target;
            transform.position = position;
            transform.forward = direction;
        }

        private void FixedUpdate()
        {
            _currentTime += Time.fixedDeltaTime;
            
            CalcMovement();
            
            if(_currentTime >= delayToRotate)
                RotateToTarget();
        }

        private void CalcMovement()
        {
            _rigidbody.linearVelocity = transform.forward * moveSpeed;
        }

        private void RotateToTarget()
        {
            Vector3 dir = _target.HitTransform.position - transform.position;
            Quaternion rotationToTarget = Quaternion.LookRotation(dir);
            Quaternion rotation = transform.rotation;

            Quaternion goalRotation = Quaternion.Lerp(rotation, rotationToTarget,Time.fixedDeltaTime * rotationSpeed);
            
            transform.rotation = goalRotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Entity entity))
            {
                if(entity.TryGetSubclassComponent(out IDamageable damageable))
                    damageable.ApplyDamage(new DamageContext());
                
                if(entity.TryGetSubclassComponent(out EntityStatusEffect statusEffect))
                    foreach (var info in bleedingBuff.GetStatusEffectInfo())
                    {
                        statusEffect.AddStatusEffect(info);
                    }
                
                Bus.Raise(new PlayEffectEvent(trackingBladeHitItemSO ,transform.position, Quaternion.LookRotation(transform.forward)));
                _myPool.Push(this);
            }
        }

        public void ResetItem()
        {
            _currentTime = 0;
        }
    }
}