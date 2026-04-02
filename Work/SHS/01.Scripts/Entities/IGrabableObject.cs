using UnityEngine;

namespace Work.AKH.Scripts.Entities
{
    public interface IGrabableObject
    {
        public Transform LeftGrabPoint { get; }
        public Transform RightGrabPoint { get; }
    }
}