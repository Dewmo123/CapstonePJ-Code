using System;
using Chipmunk.GameEvents;
using UnityEngine;
using Work.Code.UI.Core.Interaction;

namespace Work.Code.GameEvents
{
    public struct BindTooltipEvent : IEvent
    {
        public InteractableUI Owner { get; }
        public Func<object> Data { get; }
        public float Delay { get; }
        
        public BindTooltipEvent(InteractableUI owner, Func<object> data, float delay = 0f)
        {
            Owner = owner;
            Data = data;
            Delay = delay;
        }
    }

    public struct UnBindTooltipEvent : IEvent
    {
        public InteractableUI Owner { get; }

        public UnBindTooltipEvent(InteractableUI owner)
        {
            Owner = owner;
        }
    }
}