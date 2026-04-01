using Chipmunk.GameEvents;
using Code.InventorySystems.Items;
using InGame.InventorySystem;

namespace Code.GameEvents
{
    public class SwapItemEvent
    {
        public static SwapItemSlotEvent SwapItemSlotEvent = new SwapItemSlotEvent();
    }
    
    public class SwapItemSlotEvent : IEvent
    {
        public ItemSlot StartSlot;
        public ItemSlot TargetSlot;

        public bool IsHandled { get; set; }
        
        public SwapItemSlotEvent Init(ItemSlot startSlot,ItemSlot targetSlot, bool isHandled)
        {
            StartSlot = startSlot;
            TargetSlot = targetSlot;
            IsHandled = isHandled;

            return this;
        }
    }
}