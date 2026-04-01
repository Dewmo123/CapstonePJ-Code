using Chipmunk.Library.Utility.GameEvents.Local;

namespace SHS.Scripts.Crosshairs
{
    public struct CrosshairChangeEvent : ILocalEvent
    {
        public CrosshairSO CrosshairData { get; }

        public CrosshairChangeEvent(CrosshairSO crosshairData)
        {
            CrosshairData = crosshairData;
        }
    }
}