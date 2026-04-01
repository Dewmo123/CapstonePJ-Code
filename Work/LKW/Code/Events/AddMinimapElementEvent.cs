using Chipmunk.GameEvents;
using Code.UI.Minimap;
using Code.UI.Minimap.Core;
using UnityEngine;

namespace Code.Events
{
    public struct AddMinimapElementEvent : IEvent
    {
        public MinimapElementData ElementData { get; }
        public Vector3 WorldInitPos { get; }

        public AddMinimapElementEvent(MinimapElementData elementData, Vector3 worldInitPos)
        {
            ElementData = elementData;
            WorldInitPos = worldInitPos;
        }
    }
}