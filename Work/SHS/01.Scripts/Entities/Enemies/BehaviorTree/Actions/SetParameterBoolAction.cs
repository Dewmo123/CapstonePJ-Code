// using Code.SHS;
// using Code.SHS.Enemies;
// using System;
// using Code.SHS.Animations;
// using Unity.Behavior;
// using UnityEngine;
// using Action = Unity.Behavior.Action;
// using Unity.Properties;
//
// [Serializable, GeneratePropertyBag]
// [NodeDescription(name: "SetParameterBool", story: "Set [Enemy] [Parameter] to [Value]", category: "Enemy/Animation", id: "562c8d461bde93e376c4b98539749830")]
// public partial class SetParameterBoolAction : Action
// {
//     [SerializeReference] public BlackboardVariable<Enemy> Enemy;
//     [SerializeReference] public BlackboardVariable<ParameterSO> Parameter;
//     [SerializeReference] public BlackboardVariable<bool> Value;
//
//     protected override Status OnStart()
//     {
//         Enemy.Value.ParamAnimator.SetParameter(Parameter, Value.Value);
//         return Status.Success;
//     }
// }
//
