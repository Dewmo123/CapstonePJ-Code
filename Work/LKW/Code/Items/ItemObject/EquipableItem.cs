using Chipmunk.ComponentContainers;
using Work.LKW.Code.Items.ItemInfo;
using Code.SkillSystem;
using Scripts.Combat.ItemObjects;
using Scripts.Entities;
using UnityEngine;
using Work.AKH.Scripts.Entities;
using Scripts.SkillSystem.Manage;

namespace Work.LKW.Code.Items
{
    public abstract class EquipableItem : ItemBase, IEquipable
    {
        public ItemObject ItemObject;
        public SkillDataSO Skill { get; private set; }
        public EquipItemDataSO EquipItemData { get; protected set; }
        public bool IsEquipped { get; set; }
        private SkillManager _skillManager;

        public EquipableItem(ItemDataSO itemData) : base(itemData)
        {
            ItemData = itemData;
            EquipItemData = ItemData as EquipItemDataSO;
            Skill = EquipItemData.skillDB?.GetRandomSkill();
        }

        public virtual void OnEquip(Entity entity, Transform parent)
        {
            IsEquipped = true;
            GameObject go = GameObject.Instantiate(EquipItemData.equipmentPrefab, parent);
            go.transform.localPosition = EquipItemData.modelOffset;
            ItemObject = go.GetComponent<ItemObject>();
            ItemObject.InitObject(entity, this);
            _skillManager = entity.Get<SkillManager>();
            _skillManager?.AddSkill(Skill);
        }

        public virtual void OnUnequip(Entity entity)
        {
            IsEquipped = false;
            GameObject.Destroy(ItemObject.gameObject);
            ItemObject = null;
            _skillManager?.RemoveSkill(Skill);
            _skillManager = null;
        }
    }
}