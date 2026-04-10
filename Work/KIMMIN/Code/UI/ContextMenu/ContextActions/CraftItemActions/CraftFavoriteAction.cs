using UnityEngine;
using Work.Code.Crafting;

namespace Work.Code.UI.ContextMenu
{
    public class CraftFavoriteAction : BaseContextAction<CraftItemUI>
    {
        protected override string ActiveText => "즐겨찾기 해제";
        protected override string InactiveText => "즐겨찾기";

        public override bool CheckCondition(CraftItemUI data)
        {
            return data.IsFavorite;
        }

        public override void OnAction(CraftItemUI data)
        {
            data.ToggleFavorite();
        }
    }
}