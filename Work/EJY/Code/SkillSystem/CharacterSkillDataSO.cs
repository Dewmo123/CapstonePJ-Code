using System;
using System.Collections.Generic;
using Scripts.SkillSystem;
using UnityEngine;

namespace Code.SkillSystem
{
    [Serializable]
    public struct SkillInfo
    {
        public ActiveSlotType slotType;
        public SkillDataSO skillData;
    }
    
    [CreateAssetMenu(fileName = "Character Skill Data", menuName = "SO/Skill/CharacterSkillData", order = 0)]
    public class CharacterSkillDataSO : ScriptableObject
    {
        public List<SkillInfo> skills;
    }
}