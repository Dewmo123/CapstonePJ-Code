using UnityEngine;
using Work.LKW.Code.Items;

namespace Work.Code.UI.ContextMenu.InventoryItemActions
{
    public class InventoryItemUnequipAction : BaseContextAction<EquipableItem>
    {
        protected override string ActiveText { get; }
        protected override string InactiveText { get; }
        public override bool CheckCondition(EquipableItem data)
        {
            return true;
        }

        public override void OnAction(EquipableItem data)
        {
        }
    }
}