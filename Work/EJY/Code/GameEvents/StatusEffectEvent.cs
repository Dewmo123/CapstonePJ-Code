using Chipmunk.GameEvents;
using Code.StatusEffectSystem;

namespace Code.GameEvents
{
    public struct StatusEffectEvent : IEvent
    {
        public StatusEffectInfo StatusEffectInfo;

        public StatusEffectEvent(StatusEffectInfo statusEffectInfo)
        {
            StatusEffectInfo = statusEffectInfo;
        }
    }
}