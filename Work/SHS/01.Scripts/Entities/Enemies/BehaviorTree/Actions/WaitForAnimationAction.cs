// using Code.SHS.Enemies;
// using System;
// using Chipmunk.ComponentContainers;
// using Code.SHS.Animations;
// using Unity.Behavior;
// using UnityEngine;
// using Action = Unity.Behavior.Action;
// using Unity.Properties;
//
// [Serializable, GeneratePropertyBag]
// [NodeDescription(name: "WaitForAnimation", story: "[Enemy] wait for current animation end", category: "Enemy/Animation", id: "4209dce6e8c276efe35c8a0d70ee2c31")]
// public partial class WaitForAnimationAction : Action
// {
//     [SerializeReference] public BlackboardVariable<Enemy> Enemy;
//     private bool isAnimationEnded;
//     private AnimatorTrigger animatorTrigger;
//     protected override Status OnStart()
//     {
//         isAnimationEnded = false;
//         animatorTrigger = Enemy.Value.Get<AnimatorTrigger>();
//         animatorTrigger.OnAnimationEnd += HandleAnimationEnd;
//         return Status.Running;
//     }
//
//     private void HandleAnimationEnd()
//     {
//         isAnimationEnded = true;
//     }
//
//     protected override Status OnUpdate()
//     {
//         if (isAnimationEnded)
//         {
//             return Status.Success;
//         }
//         return Status.Running;
//     }
//
//     protected override void OnEnd()
//     {
//         animatorTrigger.OnAnimationEnd -= HandleAnimationEnd;
//     }
// }
//
