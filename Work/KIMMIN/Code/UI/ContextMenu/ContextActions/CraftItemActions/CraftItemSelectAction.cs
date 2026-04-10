using Work.Code.Crafting;

namespace Work.Code.UI.ContextMenu
{
    public class CraftItemSelectAction : BaseContextAction<CraftItemUI>
    {
        protected override string ActiveText => "제작";
        protected override string InactiveText => "제작";

        public override bool CheckCondition(CraftItemUI data)
        {
            return true;
        }

        public override bool CanShow(CraftItemUI data)
        {
            return true;
        }

        public override void OnAction(CraftItemUI data)
        {
            data.RequestCraft();
        }
    }
}