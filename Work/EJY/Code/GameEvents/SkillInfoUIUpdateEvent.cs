using Chipmunk.GameEvents;
using Code.SkillSystem.Upgrade;
using Scripts.SkillSystem;

namespace Code.GameEvents
{
    public struct SkillInfoUIUpdateEvent : IEvent
    {
        public Skill targetSkill;
        public SkillUpgradeSO upgradeData;

        public SkillInfoUIUpdateEvent(Skill skill, SkillUpgradeSO data)
        {
            this.targetSkill = skill;
            this.upgradeData = data;
        }
    }
}