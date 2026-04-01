using Chipmunk.GameEvents;

namespace Work.Code.GameEvents
{
    public struct PoisonStartEvent : IEvent
    {
        public int MapID { get; }

        public PoisonStartEvent(int MapID)
        {
            this.MapID = MapID;
        }
    }
}