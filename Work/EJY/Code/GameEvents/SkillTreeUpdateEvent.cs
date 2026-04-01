using System;
using System.Collections.Generic;
using Chipmunk.GameEvents;
using Scripts.SkillSystem;

namespace Code.GameEvents
{
    public class SkillTreeUpdateEvent : IEvent
    {
        public Dictionary<Type, Skill> skillDict;

        public SkillTreeUpdateEvent(Dictionary<Type, Skill> skills)
        {
            skillDict = skills;
        }
    }
}
