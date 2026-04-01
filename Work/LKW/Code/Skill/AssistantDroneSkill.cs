using DewmoLib.ObjectPool.RunTime;
using Scripts.SkillSystem;
using UnityEngine;

namespace Work.LKW.Skill
{
    public class AssistantDroneSkill : ActiveSkill
    {
        [SerializeField] private PoolManagerSO poolManager;

        public override void UseSkill()
        {
            base.UseSkill();
        }

        public override void EndSkill()
        {
            base.EndSkill();
        }
    }
}