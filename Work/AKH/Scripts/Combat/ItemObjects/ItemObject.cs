using Scripts.Combat.Fovs;
using Scripts.Entities;
using UnityEngine;
using Work.LKW.Code.Items;

namespace Scripts.Combat.ItemObjects
{
    public abstract class ItemObject : MonoBehaviour
    {
        protected Entity _owner;
        protected EquipableItem _item;
        private Renderer[] _targetRenderers;

        public virtual void InitObject(Entity owner, EquipableItem item)
        {
            _owner = owner;
            _item = item;
            if (owner is IFindable findable)
            {
                _targetRenderers = GetComponentsInChildren<Renderer>(true);
                findable.OnFound.AddListener(HandleFounded);
                HandleFounded(!findable.IsFounded);
            }
        }
        private void HandleFounded(bool arg0)
        {
            for (int i = 0; i < _targetRenderers.Length; i++)
            {
                _targetRenderers[i].forceRenderingOff = !arg0;
            }
        }
    }
}
