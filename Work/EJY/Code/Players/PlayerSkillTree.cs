using System;
using Chipmunk.ComponentContainers;
using Chipmunk.GameEvents;
using Code.GameEvents;
using Code.SkillSystem;
using Scripts.SkillSystem;
using UnityEngine;

namespace Code.Players
{
    public class PlayerSkillTree : MonoBehaviour, IContainerComponent
    {
        [field: SerializeField] public CharacterSkillDataSO CharacterSkillData { get; private set; }

        public ComponentContainer ComponentContainer { get; set; }
        
        private ActiveSkillComponent _skillComponent;
        
        public void OnInitialize(ComponentContainer componentContainer)
        {
            _skillComponent = componentContainer.Get<ActiveSkillComponent>();
        }

        private void Start()
        {
            UpdateSkillTree();
        }

        public Skill GetSkill(SkillDataSO skilData)
        {
            return _skillComponent.GetSkill(skilData);
        }
        
        public void UpdateSkillTree()
        {
            //Bus.Raise(new SkillTreeUpdateEvent(_skillComponent.Skills));
        }

        
    }
}