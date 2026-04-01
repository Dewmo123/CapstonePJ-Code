using Chipmunk.Library.Utility.GameEvents.Local;
using UnityEngine;

namespace SHS.Scripts.Combats.Events
{
    public class EntityDeadEvent : ILocalEvent
    {
        public Vector3 HitNormal { get; }
        public Vector3 HitPoint { get; }

        public EntityDeadEvent(Vector3 hitPoint, Vector3 hitNormal)
        {
            HitPoint = hitPoint;
            HitNormal = hitNormal;
        }
    }
}