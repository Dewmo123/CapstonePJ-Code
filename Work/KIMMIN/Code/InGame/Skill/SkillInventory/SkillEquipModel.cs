using System;
using System.Collections.Generic;
using Scripts.SkillSystem.Manage;
using Scripts.SkillSystem;

namespace Work.Code.SkillInventory
{
    public class SkillEquipModel
    {
        private readonly Skill[] _activeSkills = new Skill[3];
        private readonly Skill[] _passiveSkills = new Skill[3];
        
        public event Action<Skill, int> OnSkillUnequipped;
        public event Action OnSkillChanged;

        public void Equip(Skill sendSkill, Skill targetSkill, int index, SkillType type, bool isInventory)
        {
            var skills = GetSkills(type);
            Skill prevSkill = skills[index];
            int sendIndex = Array.IndexOf(skills, sendSkill);

            if (isInventory)
            {
                if (targetSkill != null)
                    SwapDuplicate(targetSkill, sendSkill, skills);
                else
                    RemoveDuplicate(sendSkill, index, skills);

                skills[index] = sendSkill;
            }
            else
            {
                skills[sendIndex] = targetSkill;
                skills[index] = sendSkill;
            }

            if (prevSkill != null && prevSkill != sendSkill)
            {
                OnSkillUnequipped?.Invoke(prevSkill, sendIndex);
            }
            
            OnSkillChanged?.Invoke();
        }
        
        private void SwapDuplicate(Skill target, Skill send, Skill[] skills)
        {
            for (int i = 0; i < skills.Length; i++)
            {
                if (skills[i] == send)
                {
                    skills[i] = target;
                    return;
                }
            }
        }

        public Skill GetSkill(int index, SkillType type)
        {
            var skills = GetSkills(type);
            if (index < 0 || index >= skills.Length) return null;
            return skills[index];
        }

        private void RemoveDuplicate(Skill skill, int except, Skill[] skills)
        {
            for (int i = 0; i < skills.Length; i++)
            {
                if (i == except) continue;
                if (skills[i] == skill)
                {
                    skills[i] = null;
                    OnSkillUnequipped?.Invoke(skill, i);
                }
            }
        }

        private Skill[] GetSkills(SkillType type)
        {
            return type == SkillType.Active ? _activeSkills : _passiveSkills;
        }
    }
}