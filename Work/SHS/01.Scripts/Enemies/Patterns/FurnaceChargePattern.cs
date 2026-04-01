// using System.Collections.Generic;
// using Code.SHS.Animations;
// using Code.SHS.Enemies.Combat.Indicators;
// using Scripts.Combat;
// using Scripts.Combat.Datas;
// using UnityEngine;
//
// namespace Code.SHS.Enemies.Patterns
// {
//     /// <summary>
//     /// 용광로 충돌 패턴 - 타겟을 향해 돌진하는 공격
//     /// </summary>
//     public class FurnaceChargePattern : BossPatternBase
//     {
//         [Header("Charge Settings")] [SerializeField]
//         private float chargeSpeed = 20f;
//
//         [SerializeField] private float chargeUpTime = 1f;
//         [SerializeField] private float chargeDuration = 2f;
//         [SerializeField] private float stunDuration = 10f;
//         [SerializeField] private float chargeWidth = 2f;
//         [SerializeField] private float stunWidth = 0.8f;
//         [SerializeField] private LayerMask targetLayer;
//         [SerializeField] private LayerMask obstacleLayer;
//         [SerializeField] private MovementDataSO knockbackData;
//
//         [Header("Animation")] [SerializeField] private ParameterSO chargeParameter;
//         [SerializeField] private ParameterSO dashParameter;
//         [Header("Effects")] [SerializeField] private AttackIndicator attackIndicator;
//         [SerializeField] private ParticleSystem chargeUpEffect;
//         [SerializeField] private ParticleSystem chargingEffect;
//         [SerializeField] private GameObject impactEffect;
//         [SerializeField] private ParticleSystem stunEffect;
//
//
//         private Vector3 _chargeDirection;
//         private Vector3 _targetPosition;
//         private bool _isCharging;
//         private Collider[] _hitColliders = new Collider[10];
//         private HashSet<Collider> _damagedColliders = new HashSet<Collider>(10);
//
//
//         protected override async void OnExecute()
//         {
//             _isCharging = false;
//             _damagedColliders.Clear();
//             // _boss.LookPlayer();
//
//             if (_boss.TargetPlayer != null)
//             {
//                 _targetPosition = _boss.TargetPlayer.transform.position;
//                 _chargeDirection = (_targetPosition - _boss.transform.position).normalized;
//                 _chargeDirection.y = 0;
//                 _boss.transform.LookAt(_targetPosition);
//             }
//
//             _boss.NavMovement.SetStop(true);
//
//             if (chargeUpEffect != null)
//                 chargeUpEffect?.Play();
//
//             ParameterAnimator.SetParameter(chargeParameter, true);
//             attackIndicator.Initialize(chargeUpTime);
//             await Awaitable.WaitForSecondsAsync(chargeUpTime);
//
//             StartDash();
//         }
//
//         protected override void OnUpdate()
//         {
//             if (_isCharging)
//             {
//                 PerformCharge();
//             }
//         }
//
//         private void StartDash()
//         {
//             _isCharging = true;
//             ParameterAnimator.SetParameter(chargeParameter, false);
//             ParameterAnimator.SetParameter(dashParameter, true);
//
//             if (chargeUpEffect != null)
//                 chargeUpEffect.Stop();
//             if (chargingEffect != null)
//                 chargingEffect.Play();
//         }
//
//         private void PerformCharge()
//         {
//             // 돌진 이동
//             Vector3 movement = _chargeDirection * chargeSpeed * Time.deltaTime;
//             _boss.transform.position += movement;
//
//             // 충돌 체크
//             CheckChargeCollision();
//
//             // 장애물 충돌 체크
//             if (Physics.SphereCast(
//                     _boss.transform.position + Vector3.up,
//                     chargeWidth,
//                     _chargeDirection,
//                     out RaycastHit hitInfo,
//                     movement.magnitude,
//                     obstacleLayer))
//             {
//                 OnHitObstacle();
//             }
//
//             // 돌진 시간 초과
//             if (_elapsedTime >= chargeUpTime + chargeDuration)
//             {
//                 _boss.EndCurrentPattern();
//             }
//         }
//
//         private void CheckChargeCollision()
//         {
//             int hitCount = Physics.OverlapSphereNonAlloc(
//                 _boss.transform.position + Vector3.up,
//                 chargeWidth,
//                 _hitColliders,
//                 targetLayer
//             );
//
//             for (int i = 0; i < hitCount; i++)
//             {
//                 Collider hitCollider = _hitColliders[i];
//                 {
//                     if (_damagedColliders.Contains(hitCollider) == false)
//                     {
//                         DamageData damage = GetCalculatedDamage(2);
//                         hitCollider.GetComponent<IDamageable>()
//                             ?.ApplyDamage(damage, hitCollider.transform.position, Vector3.zero, _boss);
//                         hitCollider.GetComponent<IKnockbackable>()?.KnockBack(_chargeDirection, knockbackData);
//
//                         _damagedColliders.Add(hitCollider);
//
//                         if (impactEffect != null)
//                         {
//                             Instantiate(impactEffect, hitCollider.transform.position, Quaternion.identity);
//                         }
//                     }
//                 }
//             }
//         }
//
//         private void OnHitObstacle()
//         {
//             if (impactEffect != null)
//             {
//                 Instantiate(impactEffect, _boss.transform.position + _chargeDirection, Quaternion.identity);
//             }
//
//             _boss.EndCurrentPattern();
//             _boss.Stun(stunDuration);
//             stunEffect?.Play();
//         }
//
//         protected override void OnEnd()
//         {
//             _isCharging = false;
//
//             if (chargeUpEffect != null)
//                 chargeUpEffect.Stop();
//             if (chargingEffect != null)
//                 chargingEffect.Stop();
//
//             ParameterAnimator.SetParameter(dashParameter, false);
//             // 잠시 경직 (선택적)
//             // _boss.ChangeState(BossStateEnum.Stagger);
//         }
//
//         private void OnDrawGizmosSelected()
//         {
//             // Gizmos.color = Color.yellow;
//             // Gizmos.DrawWireSphere(transform.position + Vector3.up, chargeWidth);
//             Gizmos.color = Color.red;
//             Gizmos.DrawWireCube(transform.position + Vector3.up,
//                 new Vector3(chargeWidth * 2, 2f, chargeWidth * 2));
//             Gizmos.color = Color.yellow;
//             Gizmos.DrawWireSphere(transform.position + Vector3.up, stunWidth);
//         }
//     }
// }