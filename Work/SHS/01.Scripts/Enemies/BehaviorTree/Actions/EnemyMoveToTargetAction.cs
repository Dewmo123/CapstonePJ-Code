// using Code.SHS.Enemies;
// using System;
// using Chipmunk.ComponentContainers;
// using Unity.Behavior;
// using UnityEngine;
// using Action = Unity.Behavior.Action;
// using Unity.Properties;
// using UnityEngine.AI;
//
// [Serializable, GeneratePropertyBag]
// [NodeDescription(name: "EnemyMoveToTarget", story: "[Enemy] move to [Target]", category: "Action/Enemy",
//     id: "2db8607b2573e4807abb8ea0d4f1f1d4")]
// public partial class EnemyMoveToTargetAction : Action
// {
//     [SerializeReference] public BlackboardVariable<Enemy> Enemy;
//     [SerializeReference] public BlackboardVariable<GameObject> Target;
//
//     protected override Status OnStart()
//     {
//         Enemy.Value.NavMovement.SetDestination(Target.Value.transform.position);
//         return Status.Success;
//     }
// }