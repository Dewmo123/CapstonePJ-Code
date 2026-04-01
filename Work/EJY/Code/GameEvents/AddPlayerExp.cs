using Chipmunk.GameEvents;

namespace Code.GameEvents
{
    public struct AddPlayerExp : IEvent
    {
        public int ExpAmount { get; private set; }

        public AddPlayerExp(int expAmount)
        {
            ExpAmount = expAmount;
        }
    }
    
    public struct ChangePlayerExp : IEvent
    {
        public int ExpAmount { get; private set; }

        public ChangePlayerExp(int expAmount)
        {
            ExpAmount = expAmount;
        }
    }
}