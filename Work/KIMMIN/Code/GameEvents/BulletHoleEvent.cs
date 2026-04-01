using Chipmunk.GameEvents;
using UnityEngine;

namespace Work.Code.GameEvents
{
    public struct BulletHoleEvent : IEvent
    {
        public Vector3 Position { get; }
        public Vector3 Normal { get; }

        public BulletHoleEvent(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }
    }
}