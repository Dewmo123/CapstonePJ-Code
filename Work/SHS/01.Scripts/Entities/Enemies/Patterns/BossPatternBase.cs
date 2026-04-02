// using System;
// using Chipmunk.ComponentContainers;
// using Code.SHS.Animations;
// using Scripts.Combat;
// using Scripts.Combat.Datas;
// using UnityEngine;
//
// namespace Code.SHS.Enemies.Patterns
// {
//     /// <summary>
//     /// 보스 패턴의 기본 클래스 - 모든 보스 패턴은 이 클래스를 상속받습니다.
//     /// </summary>
//     [Serializable]
//     public abstract class BossPatternBase : MonoBehaviour
//     {
//         [Header("Pattern Info")] [SerializeField]
//         protected string patternName;
//
//         [SerializeField, TextArea] protected string description;
//
//         [Header("Phase Availability")] [SerializeField]
//         protected bool availableInPhase1 = true;
//
//         [SerializeField] protected bool availableInPhase2 = true;
//         [SerializeField] protected bool availableInPhase3 = true;
//
//         [Header("Pattern Settings")] [SerializeField]
//         protected float baseDamage = 10f;
//
//         [SerializeField] protected float cooldown = 5f;
//         [SerializeField] protected float duration = 2f;
//         [SerializeField] protected float weight = 1f; // 패턴 선택 가중치
//
//         [Header("Range Settings")] [SerializeField]
//         protected float minRange = 0f;
//
//         [SerializeField] protected float maxRange = 20f;
//         // [SerializeField] protected ParameterSO animationTrigger;
//         protected Boss _boss;
//
//         protected float _lastExecuteTime = -999f;
//         protected float _elapsedTime;
//
//         protected DamageCalcCompo _damageCalcCompo;
//
//         public string PatternName => patternName;
//         public bool IsExecuting { get; protected set; }
//         public float Duration => duration;
//         public float ElapsedTime => _elapsedTime;
//         
//         protected ParameterAnimator ParameterAnimator => _boss.ParamAnimator;
//
//         /// <summary>
//         /// 패턴 초기화
//         /// </summary>
//         public virtual void Initialize(Boss boss)
//         {
//             _boss = boss;
//             _lastExecuteTime = -cooldown;
//             _damageCalcCompo = boss.GetCompo<DamageCalcCompo>();
//         }
//
//         /// <summary>
//         /// 해당 페이즈에서 사용 가능한지 확인
//         /// </summary>
//         public bool IsAvailableInPhase(BossPhase phase)
//         {
//             return phase switch
//             {
//                 BossPhase.Phase1 => availableInPhase1,
//                 BossPhase.Phase2 => availableInPhase2,
//                 BossPhase.Phase3 => availableInPhase3,
//                 _ => false
//             };
//         }
//
//         /// <summary>
//         /// 패턴 실행 가능 여부 (쿨다운, 거리 등 체크)
//         /// </summary>
//         public virtual bool CanExecute()
//         {
//             // 쿨다운 체크
//             if (Time.time - _lastExecuteTime < cooldown)
//                 return false;
//
//             // 거리 체크
//             if (_boss.TargetPlayer != null)
//             {
//                 float distance = Vector3.Distance(_boss.transform.position, _boss.TargetPlayer.transform.position);
//                 if (distance < minRange || distance > maxRange)
//                     return false;
//             }
//
//             return true;
//         }
//
//         /// <summary>
//         /// 패턴 선택 가중치 반환 (상황에 따라 동적으로 조절 가능)
//         /// </summary>
//         public virtual float GetWeight(Boss boss)
//         {
//             float adjustedWeight = weight;
//
//             // 페이즈에 따른 가중치 조절
//             adjustedWeight *= 1f + ((int)boss.CurrentPhase - 1) * 0.1f;
//
//             // 레이지 모드에서 가중치 증가
//             if (boss.IsInRageMode)
//                 adjustedWeight *= 1.2f;
//
//             return adjustedWeight;
//         }
//
//         /// <summary>
//         /// 패턴 실행 시작
//         /// </summary>
//         public virtual void Execute()
//         {
//             IsExecuting = true;
//             _elapsedTime = 0f;
//             _lastExecuteTime = Time.time;
//
//             // _boss.ParamAnimator.SetParameter(animationTrigger);
//
//             OnExecute();
//             Debug.Log($"[Boss Pattern] {patternName} started");
//         }
//
//         /// <summary>
//         /// 패턴 업데이트 (매 프레임 호출)
//         /// </summary>
//         public virtual void UpdatePattern()
//         {
//             if (!IsExecuting) return;
//
//             _elapsedTime += Time.deltaTime;
//             OnUpdate();
//
//             // 지속 시간 종료 체크
//             if (_elapsedTime >= duration)
//             {
//                 _boss.EndCurrentPattern();
//             }
//         }
//
//         /// <summary>
//         /// 패턴 종료
//         /// </summary>
//         public virtual void End()
//         {
//             IsExecuting = false;
//             OnEnd();
//         }
//
//         /// <summary>
//         /// 패턴별 실행 로직 (하위 클래스에서 구현)
//         /// </summary>
//         protected abstract void OnExecute();
//
//         /// <summary>
//         /// 패턴별 업데이트 로직 (하위 클래스에서 구현)
//         /// </summary>
//         protected abstract void OnUpdate();
//
//         /// <summary>
//         /// 패턴별 종료 로직 (하위 클래스에서 구현)
//         /// </summary>
//         protected abstract void OnEnd();
//
//         /// <summary>
//         /// 현재 페이즈에 따른 데미지 계산
//         /// </summary>
//         protected DamageData GetCalculatedDamage(float damageMultiplier = 1f, DamageType damageType = DamageType.None)
//         {
//             return _damageCalcCompo.CalculateDamage(
//                 baseDamage * _boss.GetDamageMultiplier()
//                 , damageMultiplier
//                 , 0
//                 , damageType);
//         }
//
//         /// <summary>
//         /// 타겟 방향으로 회전
//         /// </summary>
//         protected void LookAtTarget()
//         {
//             if (_boss.TargetPlayer != null)
//             {
//                 _boss.RotateToTarget(_boss.TargetPlayer.transform.position, true);
//             }
//         }
//
//         /// <summary>
//         /// 타겟까지의 거리
//         /// </summary>
//         protected float GetDistanceToTarget()
//         {
//             if (_boss.TargetPlayer == null) return float.MaxValue;
//             return Vector3.Distance(_boss.transform.position, _boss.TargetPlayer.transform.position);
//         }
//
//         /// <summary>
//         /// 타겟 방향 벡터
//         /// </summary>
//         protected Vector3 GetDirectionToTarget()
//         {
//             if (_boss.TargetPlayer == null) return _boss.transform.forward;
//             return (_boss.TargetPlayer.transform.position - _boss.transform.position).normalized;
//         }
//     }
// }