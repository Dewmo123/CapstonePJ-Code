using Chipmunk.ComponentContainers;
using Code.InventorySystem;
using Code.Items.ItemInfo;
using Scripts.Players;
using UnityEngine;

namespace Work.Code.Tutorials
{
    public class HotbarTutorialState : TutorialState
    {
        [SerializeField] private ItemDataSO targetItem;
        [SerializeField] private TutorialDoor tutorialDoor;
        
        private PlayerHotbar playerHotbar;
        
        public override void InitializeTutorial(TutorialController tutorialController, Player player)
        {
            base.InitializeTutorial(tutorialController, player);
            playerHotbar = player.Get<PlayerHotbar>();
        }

        public override void EnterTutorial()
        {
            base.EnterTutorial();
            playerHotbar.OnHotbarUse += HandleUseHotbar;
        }

        private void HandleUseHotbar(ItemDataSO usedItem)
        {
            if (usedItem != null && usedItem == targetItem)
            {
                TutorialComplete();
            }
        }

        public override void ExitTutorial()
        {
            playerHotbar.OnHotbarUse -= HandleUseHotbar;
            tutorialDoor.OpenDoor();
        }

        protected override string GetDialogue()
        {
            return $"[TAB]을 눌러 인벤토리를 열고 {targetItem.itemName}를 핫바에 드래그 하여 장착해 사용하세요.";
        }
    }
}