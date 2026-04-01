using Chipmunk.Library.Utility.GameEvents.Local;
using SHS.Scripts.Combats.Events;
using Unity.Cinemachine;
using UnityEngine;

namespace SHS.Scripts.Entities.Players
{
    public class GunImpulseGenerator : MonoBehaviour, ILocalEventSubscriber<GunAttackEvent>
    {
        [SerializeField] private float multiplier = 0.02f;
        [SerializeField] private CinemachineImpulseSource impulseSource;

        public void OnLocalEvent(GunAttackEvent eventData)
        {
            Vector3 direction = (eventData.Position - transform.position);
            direction.y = 0f;
            direction.Normalize();
            float force = -(eventData.GunData.horizontalRecoil + eventData.GunData.verticalRecoil) / 2 * multiplier;
                impulseSource.GenerateImpulse(direction * force);
        }
    }
}