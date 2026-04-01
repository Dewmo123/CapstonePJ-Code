using System;
using Chipmunk.GameEvents;
using UnityEngine;

namespace Work.Code.GameEvents
{
    public struct BindTooltipEvent : IEvent
    {
        public GameObject Go { get; }
        public Func<object> Data { get; }
        public float Delay { get; }
        
        public BindTooltipEvent(GameObject go, Func<object> data, float delay = 0f)
        {
            Go = go;
            Data = data;
            Delay = delay;
        }
    }

    public struct UnBindTooltipEvent : IEvent
    {
        public GameObject Go { get; }
        public Func<object> Data { get; }

        public UnBindTooltipEvent(GameObject go, Func<object> dataCallback = null)
        {
            Go = go;
            Data = dataCallback;
        }
    }
}