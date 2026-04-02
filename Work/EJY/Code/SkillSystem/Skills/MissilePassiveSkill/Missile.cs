using Chipmunk.ComponentContainers;
using Chipmunk.GameEvents;
using Code.GameEvents;
using Cysharp.Threading.Tasks;
using DewmoLib.ObjectPool.RunTime;
using Scripts.Combat;
using Scripts.Combat.Datas;
using Scripts.Effects;
using Scripts.Entities;
using UnityEngine;

namespace Code.SkillSystem.Skills.MissilePassiveSkill
{
    public class Missile : MonoBehaviour, IPoolable
    {
        [SerializeField] private OverlapDamageCaster overlapDamageCaster;
        [SerializeField] private PoolItemSO missilePoolItem;
        [SerializeField] private PoolItemSO missileExplosionPoolItem;
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private float missileSpeed = 8f;
        [SerializeField] private float searchRadius = 10f;
        [SerializeField] private float rotationSpeed = 720f;
        [SerializeField] private float bezierLookAhead = 0.08f;
        [SerializeField] private float bezierForwardFactor = 0.25f;
        [SerializeField] private float minBezierForwardDistance = 0.5f;
        [SerializeField] private float maxBezierForwardDistance = 3f;
        [SerializeField] private float minimumRiseHeight = 0.5f;
        [SerializeField] private float targetImpactDistance = 1.1f;
        [SerializeField] private float impactArmDelay = 0.05f;
        [SerializeField] private float camShakeForce = 12f;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private MeshRenderer meshRenderer;

        public PoolItemSO PoolItem => missilePoolItem;
        public GameObject GameObject => gameObject;

        private Pool _myPool;
        private Entity _owner;
        private Transform _targetTrm;
        private Transform _curveTargetTrm;
        private DamageCalcCompo _dmgCalcCompo;
        private Rigidbody _rigidbody;

        private bool _isInduction;
        private bool _isDead;
        private bool _hasFixedTargetPoint;
        private bool _isCurveActive;
        private bool _isInitialized;

        private Vector3 _lastMoveDir;
        private Vector3 _launchOffset;
        private Vector3 _initialCurveControlPoint;
        private Vector3 _fixedTargetPoint;
        private Vector3 _curveStartPoint;
        private Vector3 _curveControlPoint;
        private Vector3 _curveEndPoint;
        private float _curveProgress;
        private float _curveLength;
        private float _searchTimer;
        private float _impactArmTimer;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = false;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            if (particle != null)
            {
                var main = particle.main;
                main.simulationSpace = ParticleSystemSimulationSpace.World;

                var velocityOverLifetime = particle.velocityOverLifetime;
                velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
            }
        }

        public void InitMissile(Entity owner, Transform target, Vector3 position, bool isInduction, Vector3 launchOffset, Vector3 initialCurveControlPoint)
        {
            _owner = owner;
            _targetTrm = target;
            _curveTargetTrm = target;
            _isInduction = isInduction;
            _isDead = false;
            _isInitialized = false;
            _searchTimer = 0f;
            _impactArmTimer = impactArmDelay;
            _launchOffset = launchOffset;
            _initialCurveControlPoint = initialCurveControlPoint;
            _hasFixedTargetPoint = target != null;
            _fixedTargetPoint = target != null
                ? target.position
                : position + ((_owner != null ? _owner.transform.forward : transform.forward) * searchRadius);

            transform.position = position;
            _rigidbody.position = position;
            _rigidbody.rotation = Quaternion.identity;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            particle.Clear();
            particle.gameObject.SetActive(true);

            _dmgCalcCompo = _owner.Get<DamageCalcCompo>();
            overlapDamageCaster.InitCaster(owner);

            _lastMoveDir = _owner != null ? _owner.transform.forward : transform.forward;
            _rigidbody.rotation = Quaternion.LookRotation(_lastMoveDir);

            if (TryGetTargetPoint(out Vector3 targetPoint))
                BuildBezier(position, targetPoint, _targetTrm, _initialCurveControlPoint);
            else
                _isCurveActive = false;

            _rigidbody.linearVelocity = _lastMoveDir * missileSpeed;
            _isInitialized = true;
        }

        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }

        private void FixedUpdate()
        {
            if (_isDead || _isInitialized == false)
                return;

            if (_impactArmTimer > 0f)
                _impactArmTimer -= Time.fixedDeltaTime;

            HandleTarget();

            if (TryImpactTrackedTarget())
                return;

            if (_isCurveActive)
            {
                if (_isInduction)
                {
                    if (IsTargetValid() == false)
                    {
                        _isCurveActive = false;
                        MoveForward();
                        return;
                    }

                    if (_curveTargetTrm != _targetTrm)
                        BuildBezier(_rigidbody.position, _targetTrm.position, _targetTrm);
                }

                MoveAlongBezier();

                if (_curveProgress >= 1f)
                    _isCurveActive = false;

                TryImpactTrackedTarget();
                return;
            }

            if (_isInduction && IsTargetValid())
            {
                MoveTowardPoint(_targetTrm.position);
                TryImpactTrackedTarget();
                return;
            }

            MoveForward();
        }

        private void HandleTarget()
        {
            if (_isInduction == false)
                return;

            if (IsTargetValid())
            {
                _searchTimer = 0f;
                return;
            }

            _searchTimer += Time.fixedDeltaTime;

            if (_searchTimer >= 0.2f)
            {
                _targetTrm = FindNewTarget();
                _searchTimer = 0f;
            }
        }

        private Transform FindNewTarget()
        {
            Collider[] hits = Physics.OverlapSphere(_rigidbody.position, searchRadius, targetLayer);

            float closestDist = float.MaxValue;
            Transform closest = null;

            foreach (var hit in hits)
            {
                Entity entity = hit.GetComponentInParent<Entity>();
                if (entity == null || entity == _owner || entity.IsDead)
                    continue;

                Transform hitTransform = entity.HitTransform != null ? entity.HitTransform : entity.transform;
                float dist = Vector3.Distance(_rigidbody.position, hitTransform.position);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = hitTransform;
                }
            }

            return closest;
        }

        private bool TryGetTargetPoint(out Vector3 targetPoint)
        {
            if (_isInduction)
            {
                if (IsTargetValid())
                {
                    targetPoint = _targetTrm.position;
                    return true;
                }

                targetPoint = Vector3.zero;
                return false;
            }

            if (_hasFixedTargetPoint)
            {
                targetPoint = _fixedTargetPoint;
                return true;
            }

            targetPoint = Vector3.zero;
            return false;
        }

        private void BuildBezier(Vector3 startPoint, Vector3 endPoint, Transform targetTransform)
        {
            _curveStartPoint = startPoint;
            _curveEndPoint = endPoint;
            _curveTargetTrm = targetTransform;
            _curveProgress = 0f;
            _isCurveActive = true;

            Vector3 toTarget = endPoint - startPoint;
            Vector3 targetDir = toTarget.sqrMagnitude > 0.0001f ? toTarget.normalized : GetCurrentDirection();
            float forwardDistance = Mathf.Clamp(toTarget.magnitude * bezierForwardFactor, minBezierForwardDistance, maxBezierForwardDistance);
            float riseHeight = Mathf.Max(_launchOffset.y, minimumRiseHeight);

            _curveControlPoint = startPoint + Vector3.up * riseHeight + targetDir * forwardDistance;
            _curveLength = EstimateQuadraticBezierLength(_curveStartPoint, _curveControlPoint, _curveEndPoint);
        }

        private void BuildBezier(Vector3 startPoint, Vector3 endPoint, Transform targetTransform, Vector3 controlPoint)
        {
            _curveStartPoint = startPoint;
            _curveEndPoint = endPoint;
            _curveTargetTrm = targetTransform;
            _curveProgress = 0f;
            _isCurveActive = true;
            _curveControlPoint = controlPoint;
            _curveLength = EstimateQuadraticBezierLength(_curveStartPoint, _curveControlPoint, _curveEndPoint);
        }

        private void MoveAlongBezier()
        {
            if (_curveLength < 0.0001f)
            {
                _isCurveActive = false;
                MoveTowardPoint(_curveEndPoint);
                return;
            }

            _curveProgress = Mathf.Clamp01(_curveProgress + (missileSpeed * Time.fixedDeltaTime) / _curveLength);
            float lookAheadT = Mathf.Clamp01(_curveProgress + bezierLookAhead);
            Vector3 nextPoint = EvaluateQuadraticBezier(_curveStartPoint, _curveControlPoint, _curveEndPoint, lookAheadT);
            Vector3 desiredDir = nextPoint - _rigidbody.position;

            if (desiredDir.sqrMagnitude < 0.0001f)
                desiredDir = _curveEndPoint - _rigidbody.position;

            if (desiredDir.sqrMagnitude < 0.0001f)
            {
                TriggerImpact();
                return;
            }

            ApplyVelocity(desiredDir.normalized);
        }

        private void MoveTowardPoint(Vector3 point)
        {
            Vector3 desiredDir = point - _rigidbody.position;
            if (desiredDir.sqrMagnitude < 0.0001f)
                return;

            ApplyVelocity(desiredDir.normalized);
        }

        private void MoveForward()
        {
            ApplyVelocity(GetCurrentDirection());
        }

        private void ApplyVelocity(Vector3 desiredDir)
        {
            if (desiredDir.sqrMagnitude < 0.0001f)
                return;

            desiredDir.Normalize();
            _rigidbody.linearVelocity = desiredDir * missileSpeed;
            SmoothRotate(desiredDir, Time.fixedDeltaTime);
        }

        private Vector3 GetCurrentDirection()
        {
            if (_rigidbody.linearVelocity.sqrMagnitude > 0.0001f)
                return _rigidbody.linearVelocity.normalized;

            if (_lastMoveDir.sqrMagnitude > 0.0001f)
                return _lastMoveDir.normalized;

            if (_owner != null)
                return _owner.transform.forward;

            return transform.forward;
        }

        private bool IsTargetValid()
        {
            if (_targetTrm == null || _targetTrm.gameObject.activeInHierarchy == false)
                return false;

            Entity targetEntity = _targetTrm.GetComponentInParent<Entity>();
            if (targetEntity != null && targetEntity.IsDead)
                return false;

            return true;
        }

        private bool TryImpactTrackedTarget()
        {
            if (_impactArmTimer > 0f)
                return false;

            if (TryGetTargetPoint(out Vector3 targetPoint) == false)
                return false;

            float impactDistance = Mathf.Max(targetImpactDistance, missileSpeed * Time.fixedDeltaTime * 1.5f);
            if ((targetPoint - _rigidbody.position).sqrMagnitude > impactDistance * impactDistance)
                return false;

            _rigidbody.position = targetPoint;
            transform.position = targetPoint;
            TriggerImpact();
            return true;
        }

        private Vector3 EvaluateQuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * p0
                + 2f * oneMinusT * t * p1
                + t * t * p2;
        }

        private float EstimateQuadraticBezierLength(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            const int sampleCount = 12;
            float length = 0f;
            Vector3 previousPoint = p0;

            for (int i = 1; i <= sampleCount; i++)
            {
                float t = i / (float)sampleCount;
                Vector3 point = EvaluateQuadraticBezier(p0, p1, p2, t);
                length += Vector3.Distance(previousPoint, point);
                previousPoint = point;
            }

            return length;
        }

        private void SmoothRotate(Vector3 dir, float deltaTime)
        {
            if (dir.sqrMagnitude < 0.0001f)
                return;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            Quaternion newRot = Quaternion.RotateTowards(_rigidbody.rotation, targetRot, rotationSpeed * deltaTime);

            _rigidbody.MoveRotation(newRot);
            _lastMoveDir = newRot * Vector3.forward;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isDead || _isInitialized == false || _impactArmTimer > 0f)
                return;

            if (_owner != null && other.transform.root == _owner.transform)
                return;

            TriggerImpact();
        }

        private void TriggerImpact()
        {
            if (_isDead)
                return;

            _isDead = true;
            _isCurveActive = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _targetTrm = null;
            _curveTargetTrm = null;
            HandleImpactAsync().Forget();
        }

        private async UniTaskVoid HandleImpactAsync()
        {
            particle.Stop();

            float remaining = 0f;
            int count = particle.particleCount;
            if (count > 0)
            {
                ParticleSystem.Particle[] particleArr = new ParticleSystem.Particle[count];
                int aliveCount = particle.GetParticles(particleArr);

                if (aliveCount > 0)
                {
                    ParticleSystem.Particle lastAlive = particleArr[0];
                    for (int i = 1; i < aliveCount; i++)
                    {
                        if (particleArr[i].remainingLifetime > lastAlive.remainingLifetime)
                            lastAlive = particleArr[i];
                    }

                    remaining = lastAlive.remainingLifetime;
                }
            }

            var data = _dmgCalcCompo.CalculateDamage(8, 1, 1, DamageType.RANGE);
            overlapDamageCaster.CastDamage(data, _rigidbody.position, _lastMoveDir, null);

            if (missileExplosionPoolItem != null)
            {
                PoolingEffect effect = poolManager.Pop(missileExplosionPoolItem) as PoolingEffect;
                effect?.PlayVFX(_rigidbody.position, Quaternion.identity);
            }

            meshRenderer.enabled = false;

            Bus.Raise(new CameraShakeEvent(transform.position, Vector3.down, camShakeForce));
            await UniTask.WaitForSeconds(remaining + 0.2f);

            particle.gameObject.SetActive(false);
            _myPool.Push(this);
        }

        public void ResetItem()
        {
            meshRenderer.enabled = true;

            _isDead = false;
            _isInduction = false;
            _hasFixedTargetPoint = false;
            _isCurveActive = false;
            _isInitialized = false;
            _owner = null;
            _targetTrm = null;
            _curveTargetTrm = null;
            _dmgCalcCompo = null;
            _searchTimer = 0f;
            _impactArmTimer = 0f;
            _launchOffset = Vector3.zero;
            _initialCurveControlPoint = Vector3.zero;
            _fixedTargetPoint = Vector3.zero;
            _curveStartPoint = Vector3.zero;
            _curveControlPoint = Vector3.zero;
            _curveEndPoint = Vector3.zero;
            _curveProgress = 0f;
            _curveLength = 0f;
            _lastMoveDir = Vector3.zero;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            particle.Play();
            overlapDamageCaster.ResetRadius();
        }

        public void SetDmgRange(float radius)
        {
            overlapDamageCaster.SetRadius(overlapDamageCaster.CastRadius + radius);
        }
    }
}
