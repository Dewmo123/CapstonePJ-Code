using Chipmunk.GameEvents;

namespace Work.Code.GameEvents
{
    public struct DayChangeEvent : IEvent
    {
        public bool IsNight { get; }

        public DayChangeEvent(bool isNight)
        {
            IsNight = isNight;
        }
    }
}