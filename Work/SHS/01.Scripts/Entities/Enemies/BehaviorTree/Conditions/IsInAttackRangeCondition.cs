// using Code.SHS.Enemies;
// using DewmoLib.StatSystem;
// using System;
// using Chipmunk.ComponentContainers;
// using Scripts.Entities;
// using Unity.Behavior;
// using UnityEngine;
//
// [Serializable, Unity.Properties.GeneratePropertyBag]
// [Condition(name: "IsInAttackRange", story: "Is [Target] in [Enemy] [AttackRange]", category: "Conditions",
//     id: "5d32f6c6b60b74fcda7158989e9d125a")]
// public partial class IsInAttackRangeCondition : Condition
// {
//     [SerializeReference] public BlackboardVariable<GameObject> Target;
//     [SerializeReference] public BlackboardVariable<Enemy> Enemy;
//     [SerializeReference] public BlackboardVariable<StatSO> AttackRange;
//
//     public override bool IsTrue()
//     {
//         Enemy enemy = Enemy.Value;
//         StatSO attackRangeStat = enemy.GetContainerComponent<EntityStat>().GetStat(AttackRange);
//         return Vector3.Distance(enemy.transform.position, Target.Value.transform.position) <= attackRangeStat.Value;
//     }
//
//     public override void OnStart()
//     {
//     }
//
//     public override void OnEnd()
//     {
//     }
// }