using Chipmunk.ComponentContainers;
using Chipmunk.GameEvents;
using Code.GameEvents;
using Code.SkillSystem;
using Scripts.SkillSystem;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Work.LKW.Code.Events;

namespace Code.Players
{
    public class PlayerInfo : MonoBehaviour, IContainerComponent
    {
        [SerializeField] private PlayerLevelTableSO playerLevelTable;
        [SerializeField] private int maxThirst = 100, maxHunger = 100;
        [field: SerializeField] public int SkillPoint { get; private set; }
        [field: SerializeField] public int Level { get; private set; }
        [field: SerializeField] public int Exp { get; private set; }
        [field: SerializeField] public int Thirst { get; private set; }
        [field: SerializeField] public int Hunger { get; private set; }
        
        private PlayerSkillTree _skillTree;

        public ComponentContainer ComponentContainer { get; set; }
        public void OnInitialize(ComponentContainer componentContainer)
        {
            _skillTree = componentContainer.Get<PlayerSkillTree>();
            
            Bus.Subscribe<SkillUpgradeEvent>(HandleSkillUpgradeEvent);
            Bus.Subscribe<AddPlayerExp>(HandleAddPlayerExp);
            Bus.Subscribe<FoodIntakeEvent>(HandleTakeFood);

        }

        private void OnDestroy()
        {
            Bus.Unsubscribe<SkillUpgradeEvent>(HandleSkillUpgradeEvent);
            Bus.Unsubscribe<AddPlayerExp>(HandleAddPlayerExp);
            Bus.Unsubscribe<FoodIntakeEvent>(HandleTakeFood);
        }

        private void HandleSkillUpgradeEvent(SkillUpgradeEvent evt)
        {
            //if(SkillPoint < 1 || !evt.targetSkill.CanUpgradeSkill(evt.upgradeData)) return;

            //SkillPoint--;
            //SkillDataSO skillData = evt.targetSkill.SkillData;
            //Skill targetSkill = _skillTree.GetSkill(skillData);
            //targetSkill.UpgradeSkill(evt.upgradeData);
            //_skillTree.UpdateSkillTree();
        }

        private void HandleAddPlayerExp(AddPlayerExp evt)
        {
            Exp += evt.ExpAmount;

            if (playerLevelTable[Level] <= Exp)
            {
                Exp -= playerLevelTable[Level];
                Level++;
                SkillPoint++;
            }
            
            Bus.Raise(new ChangePlayerExp(Exp));
        }
        
            

        private void HandleTakeFood(FoodIntakeEvent evt)
        {
            Thirst += evt.WaterAmount;
            Thirst = Mathf.Clamp(Thirst, 0, maxThirst);
            Hunger += evt.FoodAmount;
            Hunger = Mathf.Clamp(Hunger, 0, maxHunger);
        }
    }
}