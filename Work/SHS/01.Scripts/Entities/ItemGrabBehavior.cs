using System;
using Chipmunk.ComponentContainers;
using Chipmunk.Library.Utility.GameEvents.Local;
using Code.GameEvents;
using InGame.InventorySystem;
using Scripts.Combat.ItemObjects;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Work.LKW.Code.Items;
using Work.SHS.Items.Events;

namespace Work.AKH.Scripts.Entities
{
    public class ItemGrabBehavior : MonoBehaviour, IContainerComponent, ILocalEventSubscriber<ItemEquippedEvent>,
        ILocalEventSubscriber<ItemUnEquippedEvent>
    {
        [SerializeField] private TwoBoneIKConstraint leftHandIK, rightHandIK;
        [SerializeField] private MultiParentConstraint itemParentConstraint;
        [SerializeField] private Transform leftHandTarget, rightHandTarget;
        [SerializeField] private Transform itemParent;
        private Rig rig;
        private GrabableObjectBehavior currentGrabableObject;

        public ComponentContainer ComponentContainer { get; set; }

        public void OnInitialize(ComponentContainer componentContainer)
        {
            rig = GetComponent<Rig>();
            rig.weight = 0;
        }

        public void OnLocalEvent(ItemEquippedEvent eventData)
        {
            rig.weight = 1;
            if (eventData.EquipableItem is EquipableItem equipableItem &&
                equipableItem.ItemObject.GrabableObjectBehavior != null)
            {
                GrabableObjectBehavior grabableObjectBehavior = equipableItem.ItemObject.GrabableObjectBehavior;
                currentGrabableObject = grabableObjectBehavior;
                if (grabableObjectBehavior.LeftGrabPoint != null)
                    leftHandIK.weight = 1;
                if (grabableObjectBehavior.RightGrabPoint != null)
                    rightHandIK.weight = 1;

                equipableItem.ItemObject.transform.SetParent(itemParent, false);
                equipableItem.ItemObject.transform.localPosition = Vector3.zero;
            }
        }

        private void Update()
        {
            if (currentGrabableObject != null)
            {
                if (currentGrabableObject.LeftGrabPoint != null)
                {
                    leftHandTarget.position = currentGrabableObject.LeftGrabPoint.position;
                    leftHandTarget.rotation = currentGrabableObject.LeftGrabPoint.rotation;
                }

                if (currentGrabableObject.RightGrabPoint != null)
                {
                    rightHandTarget.position = currentGrabableObject.RightGrabPoint.position;
                    rightHandTarget.rotation = currentGrabableObject.RightGrabPoint.rotation;
                }
            }
        }

        public void OnLocalEvent(ItemUnEquippedEvent eventData)
        {
            rig.weight = 0;
            leftHandIK.weight = 0;
            rightHandIK.weight = 0;
        }

        public void SetWeight(int i)
        {
            Debug.Log("SetWeight: " + i);
            rig.weight = i;
        }
    }
}