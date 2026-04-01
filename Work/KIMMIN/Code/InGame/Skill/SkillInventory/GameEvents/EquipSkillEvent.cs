using Chipmunk.GameEvents;
using Chipmunk.Library.Utility.GameEvents.Local;
using Scripts.SkillSystem;

namespace Work.Code.SkillInventory.GameEvents
{
    public struct EquipSkillEvent : ILocalEvent
    {
        public Skill EquippedSkill { get; }
        public int Index { get; }

        public EquipSkillEvent(Skill equippedSkill, int index)
        {
            EquippedSkill = equippedSkill;
            Index = index;
        }
    }

    public struct UnEquipSkillEvnt : ILocalEvent
    {
        public Skill UnEquippedSkill { get; }
        public int Index { get; }

        public UnEquipSkillEvnt(Skill unEquippedSkill, int index)
        {
            UnEquippedSkill = unEquippedSkill;
            Index = index;
        }
    }
}