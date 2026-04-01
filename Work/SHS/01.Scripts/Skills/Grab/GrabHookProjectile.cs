using Code.SHS.Entities.Enemies.Combat;
using Cysharp.Threading.Tasks;
using Scripts.Combat;
using Scripts.Combat.Datas;
using Scripts.Entities;
using UnityEngine;

namespace Scripts.SkillSystem.Skills.Grab
{
    public class GrabHookProjectile : MonoBehaviour
    {
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        private Entity _owner;
        private Transform _pullAnchor;
        private Vector3 _direction;
        private LayerMask _hitMask;
        private float _speed;
        private float _maxDistance;
        private float _sphereRadius;
        private float _lifeTime;
        private float _pullStopDistance;
        private float _controlLockDuration;
        private float _traveledDistance;
        private float _elapsedTime;
        private bool _initialized;
        private bool _resolved;

        private MovementDataSO _pullMovementData;
        private DamageData _damageData;

        public void Launch(
            Entity owner,
            Transform pullAnchor,
            Vector3 direction,
            LayerMask hitMask,
            float speed,
            float maxDistance,
            float sphereRadius,
            float lifeTime,
            MovementDataSO pullMovementData,
            float pullStopDistance,
            float controlLockDuration,
            DamageData damageData)
        {
            _owner = owner;
            _pullAnchor = pullAnchor;
            _direction = direction.normalized;
            _hitMask = hitMask;
            _speed = Mathf.Max(0.01f, speed);
            _maxDistance = Mathf.Max(0.1f, maxDistance);
            _sphereRadius = Mathf.Max(0.01f, sphereRadius);
            _lifeTime = Mathf.Max(0.05f, lifeTime);
            _pullMovementData = pullMovementData;
            _pullStopDistance = Mathf.Max(0f, pullStopDistance);
            _controlLockDuration = Mathf.Max(0f, controlLockDuration);
            _damageData = damageData;
            _traveledDistance = 0f;
            _elapsedTime = 0f;
            _initialized = true;
            _resolved = false;

            if (_direction.sqrMagnitude < 0.0001f)
                _direction = transform.forward;

            transform.forward = _direction;
        }

        private void Update()
        {
            if (!_initialized || _resolved)
                return;

            float step = _speed * Time.deltaTime;
            Vector3 origin = transform.position;

            if (Physics.SphereCast(
                    origin,
                    _sphereRadius,
                    _direction,
                    out RaycastHit hit,
                    step,
                    _hitMask,
                    triggerInteraction))
            {
                if (TryResolveHit(hit))
                    return;
            }

            transform.position = origin + _direction * step;
            _traveledDistance += step;
            _elapsedTime += Time.deltaTime;

            if (_traveledDistance >= _maxDistance || _elapsedTime >= _lifeTime)
            {
                Destroy(gameObject);
            }
        }

        private bool TryResolveHit(RaycastHit hit)
        {
            Entity targetEntity = hit.collider.GetComponentInParent<Entity>();

            if (targetEntity != null)
            {
                if (_owner != null && targetEntity == _owner)
                    return false;

                _resolved = true;
                HandleEntityHit(targetEntity, hit.point, hit.normal);
                Destroy(gameObject);
                return true;
            }

            _resolved = true;
            Destroy(gameObject);
            return true;
        }

        private void HandleEntityHit(Entity targetEntity, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (targetEntity.TryGetComponent(out IDamageable damageable))
            {
                DamageContext context = new DamageContext
                {
                    DamageData = _damageData,
                    HitPoint = hitPoint,
                    HitNormal = hitNormal,
                    Source = targetEntity.gameObject,
                    Attacker = targetEntity
                };
                    
                damageable.ApplyDamage(context);
                _owner?.OnHit?.Invoke(_owner, damageable);
            }

            PullTargetAsync(targetEntity).Forget();
        }

        private async UniTaskVoid PullTargetAsync(Entity targetEntity)
        {
            if (_pullMovementData == null || targetEntity == null)
                return;

            ISkillMovement skillMovement = targetEntity.GetComponent<ISkillMovement>();
            skillMovement ??= targetEntity.GetComponentInChildren<ISkillMovement>();

            if (skillMovement == null)
                return;

            float lockDuration = Mathf.Max(_controlLockDuration, _pullMovementData.duration);
            ApplyControlLock(targetEntity, lockDuration);

            Vector3 anchorPos = _pullAnchor != null
                ? _pullAnchor.position
                : _owner != null
                    ? _owner.transform.position
                    : targetEntity.transform.position;

            Vector3 pullDirection = anchorPos - targetEntity.transform.position;
            pullDirection.y = 0f;

            if (pullDirection.sqrMagnitude < 0.0001f)
                return;

            float distanceToAnchor = pullDirection.magnitude;
            if (distanceToAnchor <= _pullStopDistance)
                return;

            pullDirection /= distanceToAnchor;

            bool prevCanMove = skillMovement.CanMove;
            skillMovement.CanMove = false;
            skillMovement.SetRotation(pullDirection);
            skillMovement.ApplyMovementData(pullDirection, _pullMovementData);

            await UniTask.WaitForSeconds(_pullMovementData.duration);

            if (skillMovement is Component movementComponent && movementComponent != null)
            {
                skillMovement.CanMove = prevCanMove;
            }
        }

        private static void ApplyControlLock(Entity targetEntity, float duration)
        {
            if (targetEntity is IStunable stunable)
            {
                stunable.Stun(duration);
            }
        }
    }
}
