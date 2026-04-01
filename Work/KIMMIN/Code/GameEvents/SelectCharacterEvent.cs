using Chipmunk.GameEvents;
using UnityEngine;
using Work.Code.Setting;
using Work.Code.SkillTree;

namespace Work.Code.GameEvents
{
    public struct SelectCharacterEvent : IEvent
    {
        public ChrarcterSO Character { get; }

        public SelectCharacterEvent(ChrarcterSO character)
        {
            this.Character = character;
        }
    }
}