using UnityEngine;
using Work.LKW.Code.Items;

namespace Work.Code.UI.ContextMenu.InventoryItemActions
{
    public class InventoryItemUseAction : BaseContextAction<UsableItem>
    {
        protected override string ActiveText { get; }
        protected override string InactiveText { get; }
        public override bool CheckCondition(UsableItem data)
        {
            return true;
        }

        public override void OnAction(UsableItem data)
        {
        }
    }
}