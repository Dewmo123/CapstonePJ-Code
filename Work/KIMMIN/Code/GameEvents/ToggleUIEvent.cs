using Chipmunk.GameEvents;
using Code.UI.Core;
using UnityEngine;

namespace InGame.PlayerUI
{
    public struct ToggleUIEvent : IEvent
    {
        public UIPanel Stack { get; }
        public bool IsActive { get; }

        public ToggleUIEvent(UIPanel ui, bool isActive = false)
        {
            Stack = ui;
            IsActive = isActive;
        }
    }

    public struct ChangeCursorEvent : IEvent
    {
        public bool IsLocked { get; }

        public ChangeCursorEvent(bool isLocked = false)
        {
            IsLocked = isLocked;
        }
    }
}