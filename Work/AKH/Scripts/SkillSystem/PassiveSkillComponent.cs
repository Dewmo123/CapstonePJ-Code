using Chipmunk.ComponentContainers;
using Scripts.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.SkillSystem
{
    public enum PassiveSlotType
    {
        Passive1,
        Passive2,
        Passive3,
    }

    public class PassiveSkillComponent : SkillComponent<PassiveSlotType,PassiveSkillSocket>
    {
        public sealed override SkillType SkillType => SkillType.Passive;
    }
}
