using Chipmunk.GameEvents;
using DewmoLib.ObjectPool.RunTime;
using UnityEngine;

namespace Code.GameEvents
{
    public struct PlayEffectEvent : IEvent
    {
        public PoolItemSO poolItem;
        public Vector3 position;
        public Quaternion rotation;
    }
}