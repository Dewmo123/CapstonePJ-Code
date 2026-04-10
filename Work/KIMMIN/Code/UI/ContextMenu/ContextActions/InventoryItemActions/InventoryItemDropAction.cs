using UnityEngine;
using Work.LKW.Code.Items;

namespace Work.Code.UI.ContextMenu.InventoryItemActions
{
    public class InventoryItemDropAction : BaseContextAction<ItemBase>
    {
        protected override string ActiveText { get; }
        protected override string InactiveText { get; }
        public override bool CheckCondition(ItemBase data)
        {
            return true;
        }

        public override void OnAction(ItemBase data)
        {
            
        }
    }
}