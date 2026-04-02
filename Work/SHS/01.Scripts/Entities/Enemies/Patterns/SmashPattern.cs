// using System;
// using Chipmunk.ComponentContainers;
// using Code.SHS.Animations;
// using Code.SHS.Enemies.Combat.Indicators;
// using Scripts.Combat;
// using Scripts.Combat.Datas;
// using Scripts.Entities;
// using UnityEngine;
// using UnityEngine.Serialization;
//
// namespace Code.SHS.Enemies.Patterns
// {
//     public class SmashPattern : BossPatternBase
//     {
//         [Header("Smash Settings")] [SerializeField]
//         private float smashRadius = 5f;
//
//         [SerializeField] private float smashDamageMultiplier = 3f;
//         [Header("Animation")] [SerializeField] private ParameterSO smashParameter;
//
//         [Header("Effects")] [SerializeField] private GameObject smashEffectPrefab;
//         private Collider[] _hitColliders = new Collider[10];
//         [SerializeField] private AttackIndicator _attackIndicator;
//
//         protected override void OnExecute()
//         {
//             _boss.LookPlayer();
//             ParameterAnimator.SetParameter(smashParameter, true);
//             _boss.NavMovement.SetStop(true);
//
//             _boss.Get<EntityAnimatorTrigger>().OnDamageCastTrigger += Attack;
//             _attackIndicator.Initialize(0.8f);
//         }
//
//         protected override void OnUpdate()
//         {
//         }
//
//         protected override void OnEnd()
//         {
//             ParameterAnimator.SetParameter(smashParameter, false);
//             _boss.NavMovement.SetStop(false);
//
//             _boss.Get<EntityAnimatorTrigger>().OnDamageCastTrigger -= Attack;
//         }
//
//         public void Attack()
//         {
//             if (smashEffectPrefab != null)
//             {
//                 Debug.Log("Smash Effect Instantiate");
//                 ;
//                 Instantiate(smashEffectPrefab, transform.position, Quaternion.identity);
//             }
//
//             int hitCount = Physics.OverlapSphereNonAlloc(_boss.transform.position, smashRadius, _hitColliders,
//                 _boss.playerLayerMask);
//             for (int i = 0; i < hitCount; i++)
//             {
//                 Collider hitCollider = _hitColliders[i];
//                 IDamageable health = hitCollider.GetComponent<IDamageable>();
//                 if (health != null)
//                 {
//                     DamageData damageData = GetCalculatedDamage(smashDamageMultiplier);
//                     health.ApplyDamage(damageData, _boss);
//                 }
//                 else
//                 {
//                     Debug.Log($"{hitCollider} 에 IDamageable없는듯");
//                 }
//             }
//
//             _attackIndicator.Complete(CompleteAction.Deactivate);
//             _boss.EndCurrentPattern();
//         }
//
//         private void OnDrawGizmosSelected()
//         {
//             Gizmos.color = Color.red;
//             Gizmos.DrawWireSphere(transform.position, smashRadius);
//         }
//     }
// }