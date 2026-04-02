using System;
using System.Collections.Generic;
using Chipmunk.ComponentContainers;
using Code.SHS.Animations;
using Code.SHS.Entities.Enemies.FSM;
using Scripts.Combat;
using Scripts.Combat.Datas;
using Scripts.Entities;
using Scripts.FSM;
using Scripts.Players;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Code.SHS.Entities.Enemies
{
    public class Boss : Enemy
    {
        public ParameterAnimator ParamAnimator { get; private set; }

        public override void OnInitialize(ComponentContainer componentContainer)
        {
            base.OnInitialize(componentContainer);
            ParamAnimator = ComponentContainer.GetComponent<ParameterAnimator>();
        }
    }
}
