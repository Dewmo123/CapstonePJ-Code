using Chipmunk.GameEvents;
using Code.InventorySystems.Items;
using Code.Players;
using Work.LKW.Code.Items;
using Work.LKW.Code.Items.ItemInfo;

namespace InGame.InventorySystem
{
    public struct EquipItemEvent : IEvent
    {
        public ItemSlot ItemSlot { get; }
        public EquipSlotType Type { get; }
        
        public EquipItemEvent(ItemSlot itemSlot, EquipSlotType type)
        {
            this.ItemSlot = itemSlot;
            Type = type;
        }
    }
    
    public struct UnEquipItemEvent : IEvent
    {
        public ItemSlot ItemSlot { get; }
        public EquipSlotType Type { get; }
        
        public UnEquipItemEvent(ItemSlot itemSlot, EquipSlotType type)
        {
            this.ItemSlot = itemSlot;
            Type = type;
        }
    }

    public struct EquipByDragEvent : IEvent
    {
        public ItemBase Item { get; }
        public EquipSlotType Type { get; }
        public ItemSlot StartSlot { get; }

        public EquipByDragEvent(ItemBase item, EquipSlotType type, ItemSlot startSlot)
        {
            this.Item = item;
            Type = type;
            StartSlot = startSlot;
        }
    }
    
    public struct UnEquipByDragEvent : IEvent
        {
            public ItemBase Item { get; }
            public ItemSlot TargetSlot { get; }
    
            public UnEquipByDragEvent(ItemBase item,ItemSlot targetSlot)
            {
                this.Item = item;
                this.TargetSlot = targetSlot;
            }
        }
}