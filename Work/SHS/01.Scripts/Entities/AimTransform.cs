using System;
using Chipmunk.ComponentContainers;
using Code.ETC;
using UnityEngine;

namespace Work.AKH.Scripts.Entities
{
    public class AimTransform : MonoBehaviour, IContainerComponent
    {
        private IAimProvider _aimProvider;
        public ComponentContainer ComponentContainer { get; set; }

        public void OnInitialize(ComponentContainer componentContainer)
        {
            ComponentContainer = componentContainer;
            _aimProvider = componentContainer.GetSubclassComponent<IAimProvider>();
        }

        private void LateUpdate()
            => transform.position = _aimProvider.GetAimPosition();
    }
}