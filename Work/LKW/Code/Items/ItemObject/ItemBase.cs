using System;
using Code.InventorySystems;
using Work.LKW.Code.Items.ItemInfo;
using UnityEngine;

namespace Work.LKW.Code.Items
{
    [Serializable]
    public abstract class ItemBase
    {
        [field: SerializeField] public ItemDataSO ItemData { get; protected set; }

        public ItemBase(ItemDataSO itemData)
        {
            ItemData = itemData;
        }
    }
}