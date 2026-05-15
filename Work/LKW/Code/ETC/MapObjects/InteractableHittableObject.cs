using Code.ETC.MapObjects;
using EPOOutline;
using Scripts.Entities;
using UnityEngine;
using Work.Code.UI;
using Work.LKW.Code.ItemContainers;

namespace Code.ETC.MapObjects
{
    public abstract class InteractableHittableObject : HittableObject, IInteractable
    {
        [SerializeField] private AppearEffect helpText;
        [field: SerializeField] public Outlinable Outlinable { get; private set; }

        private Camera _cam;
        private bool _isSelected;

        protected override void Awake()
        {
            base.Awake();
            _cam = Camera.main;
        }

        protected virtual void Start()
        {
            Outlinable.enabled = false;
            helpText.Disappear();
        }

        private void LateUpdate()
        {
            if (_isSelected)
                helpText.transform.forward = _cam.transform.forward;
        }

        public void Select()
        {
            if (_isSelected) return;
            _isSelected = true;
            helpText.Appear();
            Outlinable.enabled = true;
        }

        public void DeSelect()
        {
            helpText.Disappear();
            _isSelected = false;
            Outlinable.enabled = false;
        }

        public override void TakeHit()
        {
            base.TakeHit();
        }

        public abstract void Interact(Entity interactor);
    }
}
