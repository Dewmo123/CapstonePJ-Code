using System;
using Chipmunk.GameEvents;
using Work.Code.UI.Core.Interaction;

namespace Work.Code.UI.ContextMenu
{
    public struct BindContextMenuEvent : IEvent
    {
        public InteractableUI Owner { get; }
        public ContextMenuSO ContextMenu { get; }
        public Func<object> Data { get; }

        public BindContextMenuEvent(InteractableUI owner, ContextMenuSO contextMenu, Func<object> data)
        {
            Owner = owner;
            ContextMenu = contextMenu;
            Data = data;
        }
    }

    public struct UnBindContextMenuEvent : IEvent
    {
        public InteractableUI Owner { get; }
        public UnBindContextMenuEvent(InteractableUI owner)
        {
            Owner = owner;
        }
    }
}