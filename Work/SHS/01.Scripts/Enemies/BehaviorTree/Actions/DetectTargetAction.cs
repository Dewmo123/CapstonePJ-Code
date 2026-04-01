// using System;
// using Chipmunk.ComponentContainers;
// using Code.SHS.Enemies;
// using DewmoLib.StatSystem;
// using Scripts.Entities;
// using Unity.Behavior;
// using Unity.Properties;
// using UnityEngine;
// using Action = Unity.Behavior.Action;
//
// namespace Code.SHS.Enemies.BehaviorTree.Actions
// {
//     [Serializable, GeneratePropertyBag]
//     [NodeDescription(name: "Detect Target", story: "[Enemy] detect [Target] in [DetectRange]", category: "Enemy/Combat",
//         id: "04cea5e3c9604931eb7f327a9f7076fe")]
//     public partial class DetectTargetAction : Action
//     {
//         [SerializeReference] public BlackboardVariable<Enemy> Enemy;
//         [SerializeReference] public BlackboardVariable<GameObject> Target;
//         [SerializeReference] public BlackboardVariable<StatSO> DetectRange;
//
//         private Enemy _enemy;
//         private StatSO _runtimeStat;
//         private Collider[] _detectedColliders = new Collider[10];
//
//         protected override Status OnStart()
//         {
//             if (_enemy == null || _runtimeStat == null)
//             {
//                 _enemy = Enemy.Value;
//                 _runtimeStat = _enemy.GetContainerComponent<EntityStat>().GetStat(DetectRange);
//             }
//
//             return OnUpdate();
//         }
//
//         protected override Status OnUpdate()
//         {
//             int cnt = Physics.OverlapSphereNonAlloc(_enemy.transform.position, _runtimeStat.Value, _detectedColliders,
//                 _enemy.playerLayerMask);
//             if (cnt == 0)
//                 return Status.Running;
//             Target.Value = _detectedColliders[0].gameObject;
//             return Status.Success;
//         }
//
//         protected override void OnEnd()
//         {
//         }
//     }
// }