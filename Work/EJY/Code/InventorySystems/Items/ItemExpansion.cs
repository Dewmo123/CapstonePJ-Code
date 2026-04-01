using System;
using Code.Players;
using Work.LKW.Code.Items.ItemInfo;

namespace Code.InventorySystems.Items
{
    public static class ItemExpansion
    {
        public static EquipSlotType GetEquipSlotType(this ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.MeleeWeapon:
                    return EquipSlotType.Melee;
                case ItemType.Gun:
                case ItemType.Throw:
                    return EquipSlotType.Weapon1;
                case ItemType.Armor:
                    return EquipSlotType.Armor;
                case ItemType.Helmet:
                    return EquipSlotType.Helmet;
                default:
                    return EquipSlotType.None;
            }
        }
        
        public static bool IsAssignableTo(this ItemType itemType, EquipSlotType slotType)
        {
            switch (itemType)
            {
                case ItemType.Gun:
                    return slotType == EquipSlotType.Weapon1 || slotType == EquipSlotType.Weapon2;

                case ItemType.MeleeWeapon:
                    return slotType == EquipSlotType.Melee;

                case ItemType.Armor:
                    return slotType == EquipSlotType.Armor;

                case ItemType.Helmet:
                    return slotType == EquipSlotType.Helmet;

                default:
                    return false;
            }
        }

        public static EquipType GetEquipType(this EquipSlotType equipSlotType)
        {
            switch (equipSlotType)
            {
                case EquipSlotType.None:
                    return EquipType.None;
                case EquipSlotType.Weapon1:
                    return EquipType.Hand;
                case EquipSlotType.Weapon2:
                    return EquipType.Hand;
                case EquipSlotType.Melee:
                    return EquipType.Hand;
                case EquipSlotType.Helmet:
                    return EquipType.Helmet;
                case EquipSlotType.Armor:
                    return EquipType.Armor;
                default:
                    return EquipType.None;
            }
        }
    }
    
    
}