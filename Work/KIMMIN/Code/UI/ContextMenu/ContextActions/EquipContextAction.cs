using System;
using UnityEngine;
using Work.LKW.Code.Items;

namespace Work.Code.UI.ContextMenu
{
    public class EquipContextAction : BaseContextAction<ItemBase>
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