using System;
using Chipmunk.ComponentContainers;
using Cysharp.Threading.Tasks;
using DewmoLib.ObjectPool.RunTime;
using Scripts.Combat;
using Scripts.Combat.Datas;
using Scripts.Effects;
using Scripts.Entities;
using UnityEngine;
using UnityEngine.Splines;

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
        [SerializeField] private float rotationSpeed = 360f;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private MeshRenderer meshRenderer;

        private float _currentLifeTime = 0;

        public PoolItemSO PoolItem => missilePoolItem;
        public GameObject GameObject => gameObject;

        private Pool _myPool;
        private Entity _owner;
        private Transform _targetTrm;
        private DamageCalcCompo _dmgCalcCompo;
        private Rigidbody _rigidbody;

        private bool _isInduction = false;
        private bool _isDead = false;

        private Vector3 _lastMoveDir;

        #region Path

        private Spline _path;
        private float _ratio;
        private float _pathLength;
        private float _time;

        #endregion

        private float _searchTimer;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.isKinematic = true;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public void InitMissile(Entity owner, Transform target, Vector3 position, bool isInduction)
        {
            _owner = owner;
            _targetTrm = target;
            _isInduction = isInduction;

            particle.Clear();
            
            _rigidbody.position = position;
            _rigidbody.rotation = Quaternion.identity;

            _dmgCalcCompo = _owner.Get<DamageCalcCompo>();
            overlapDamageCaster.InitCaster(owner);

            _ratio = 0;
            _currentLifeTime = 0;
            _searchTimer = 0f;

            _lastMoveDir = transform.forward;
        }

        private void SmoothRotate(Vector3 dir)
        {
            if (dir.sqrMagnitude < 0.0001f) return;
        
            Quaternion targetRot = Quaternion.LookRotation(dir);
        
            Quaternion newRot = Quaternion.RotateTowards(
                _rigidbody.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        
            _rigidbody.MoveRotation(newRot);
            _lastMoveDir = newRot * Vector3.forward;
        }
        
        public void SetPathToTarget(Spline path)
        {
            _path = path;
            _pathLength = path.GetLength();
            _time = Mathf.Max(0.01f, _pathLength / missileSpeed);
        }

        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }

        private void Update()
        {
            if (_isDead) return;

            HandleTarget();

            if (_targetTrm != null)
            {
                if (_isInduction)
                {
                    MoveHoming();
                    return;
                }
                else
                {
                    MoveAlongSpline();
                    return;
                }
            }

            MoveForward();
        }

        #region Target Logic

        private void HandleTarget()
        {
            if (_targetTrm == null || !_targetTrm.gameObject.activeInHierarchy)
            {
                _searchTimer += Time.deltaTime;

                if (_searchTimer >= 0.2f)
                {
                    _targetTrm = FindNewTarget();
                    _searchTimer = 0f;
                }
            }
        }

        private Transform FindNewTarget()
        {
            Collider[] hits = Physics.OverlapSphere(_rigidbody.position, searchRadius, targetLayer);

            float closestDist = float.MaxValue;
            Transform closest = null;

            foreach (var hit in hits)
            {
                Entity entity = hit.GetComponent<Entity>();
                if (entity == null || entity == _owner) continue;

                float dist = Vector3.Distance(_rigidbody.position, hit.transform.position);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = hit.transform;
                }
            }

            return closest;
        }

        #endregion

        #region Movement

        private void MoveAlongSpline()
        {
            if (_path == null)
            {
                MoveForward();
                return;
            }

            _currentLifeTime += Time.deltaTime;
            _ratio = Mathf.Clamp01(_currentLifeTime / _time);

            Vector3 targetPos = _path.EvaluatePosition(_ratio);
            Vector3 tangent = _path.EvaluateTangent(_ratio);

            if (tangent.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(tangent);
                _rigidbody.MoveRotation(targetRot);
                _lastMoveDir = targetRot * Vector3.forward;
            }

            _rigidbody.MovePosition(targetPos);

            if (_ratio >= 1f)
            {
                _path = null;
            }
        }

        private void MoveHoming()
        {
            if (_targetTrm == null)
            {
                MoveForward();
                return;
            }

            Vector3 dir = (_targetTrm.position - _rigidbody.position).normalized;

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                Quaternion newRot = Quaternion.Slerp(_rigidbody.rotation, targetRot, Time.deltaTime * 10f);

                _rigidbody.MoveRotation(newRot);
                _lastMoveDir = newRot * Vector3.forward;
            }

            Vector3 nextPos = _rigidbody.position + _lastMoveDir * missileSpeed * Time.deltaTime;
            _rigidbody.MovePosition(nextPos);
        }

        private void MoveForward()
        {
            Vector3 nextPos = _rigidbody.position + _lastMoveDir * missileSpeed * Time.deltaTime;
            _rigidbody.MovePosition(nextPos);
        }

        #endregion

        private async void OnTriggerEnter(Collider other)
        {
            if (_isDead) return;
            _isDead = true;

            particle.Stop();

            int count = particle.particleCount;
            ParticleSystem.Particle[] particleArr = new ParticleSystem.Particle[count];

            int aliveCount = particle.GetParticles(particleArr);

            if (aliveCount == 0)
                return;

            ParticleSystem.Particle lastAlive = particleArr[0];

            for (int i = 1; i < aliveCount; i++)
            {
                if (particleArr[i].remainingLifetime > lastAlive.remainingLifetime)
                    lastAlive = particleArr[i];
            }

            float remaining = lastAlive.remainingLifetime;

            var data = _dmgCalcCompo.CalculateDamage(8, 1, 1, DamageType.RANGE);
            overlapDamageCaster.CastDamage(data, _rigidbody.position, _lastMoveDir, null);

            _path = null;
            _targetTrm = null;

            if (missileExplosionPoolItem != null)
            {
                PoolingEffect effect = poolManager.Pop(missileExplosionPoolItem) as PoolingEffect;
                effect?.PlayVFX(_rigidbody.position, Quaternion.identity);
            }

            meshRenderer.enabled = false;

            await UniTask.WaitForSeconds(remaining + 0.2f);

            _myPool.Push(this);
        }

        public void ResetItem()
        {
            meshRenderer.enabled = true;

            _isDead = false;
            _currentLifeTime = 0;
            _ratio = 0;
            _targetTrm = null;
            _path = null;
            _searchTimer = 0f;

            particle.Play();
            overlapDamageCaster.ResetRadius();
        }

        public void SetDmgRange(float radius)
        {
            overlapDamageCaster.SetRadius(overlapDamageCaster.CastRadius + radius);
        }
    }
}