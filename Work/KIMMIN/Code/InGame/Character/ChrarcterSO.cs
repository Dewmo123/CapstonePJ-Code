using UnityEngine;
using Work.Code.SkillTree;

namespace Work.Code.Setting
{
    [CreateAssetMenu(fileName = "Character", menuName = "SO/Character", order = 0)]
    public class ChrarcterSO : ScriptableObject
    {
        public string characterName;
        [TextArea]
        public string description;
        public Sprite characterIcon;
        public Color characterColor;
        public CharacterType characterType;
    }
}