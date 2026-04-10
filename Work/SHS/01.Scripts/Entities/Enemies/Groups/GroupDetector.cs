using Chipmunk.ComponentContainers;
using SHS.Scripts.Entities.Players;
using UnityEngine;

namespace Code.SHS.Entities.Enemies.Groups
{
    public class GroupDetector : MonoBehaviour, IContainerComponent
    {
        private EntitySensor _sensor;
        public ComponentContainer ComponentContainer { get; set; }

        public void OnInitialize(ComponentContainer componentContainer)
        {
            _sensor = componentContainer.Get<EntitySensor>();
        }
    }
}