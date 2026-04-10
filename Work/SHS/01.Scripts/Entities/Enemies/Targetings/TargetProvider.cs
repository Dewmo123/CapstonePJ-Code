using System;
using Chipmunk.ComponentContainers;
using Chipmunk.Library.Utility.GameEvents.Local;
using Code.SHS.Entities.Enemies.Events;
using Code.SHS.Entities.Enemies.Targetings.Events;
using Scripts.Entities;
using UnityEngine;

namespace Code.SHS.Targetings.Enemies
{
    public class TargetProvider : MonoBehaviour, IContainerComponent
    {
        [SerializeField] private float targetForgetDuration = 5f;
        public Entity CurrentTarget => _currentTarget;
        public Entity Target => _target;
        private Entity _currentTarget;
        private Entity _target;
        public Vector3 LastTargetPosition => _lastPosition;
        private Vector3 _lastPosition;
        private float _targetForgetTimer = 0f;

        private LocalEventBus _localEventBus;
        public ComponentContainer ComponentContainer { get; set; }

        public void OnInitialize(ComponentContainer componentContainer)
        {
            _localEventBus = componentContainer.Get<LocalEventBus>();
        }

        private void Update()
        {
            UpdateTargetRemember();
            UpdateLastTargetPosition();
        }

        private void UpdateTargetRemember()
        {
            if (_currentTarget != null && _target == null)
            {
                _targetForgetTimer += Time.deltaTime;
                if (_targetForgetTimer >= targetForgetDuration)
                {
                    _currentTarget = null;
                    TargetLost(_lastPosition);
                }
            }
        }

        private void UpdateLastTargetPosition()
        {
            if (_target == null) return;
            _lastPosition = _target.transform.position;
        }

        public float GetTargetDistance()
        {
            if (CurrentTarget == null)
                return float.MaxValue;
            return Vector3.Distance(transform.position, CurrentTarget.transform.position);
        }

        public void SetTarget(Entity target)
        {
            Entity previousTarget = _currentTarget;
            _target = target;
            if (target != null)
            {
                if (previousTarget == null)
                    _localEventBus.Raise(new TargetDetectedEvent(target));
                _currentTarget = target;
            }
            else if (target == null && previousTarget != null)
            {
                TargetLost(_lastPosition);
            }
        }

        public void TargetLost(Vector3 lastKnownPosition)
        {
            if (CurrentTarget != null) return;
            _lastPosition = lastKnownPosition;
            _targetForgetTimer = 0f;
            _localEventBus.Raise(new TargetLostEvent(lastKnownPosition));
        }
    }
}