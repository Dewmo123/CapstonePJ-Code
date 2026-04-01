using Code.UI.Core;
using Scripts.SkillSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.Code.SkillInventory;

namespace InGame.InventorySystem
{
    public class SkillUpgradeUI : UIBase, IUIElement<Skill>
    {
        [SerializeField] private SkillSlot skillSlot;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button upgradeButton;

        private Skill _skill;
        
        protected override void Awake()
        {
            base.Awake();
            upgradeButton.onClick.AddListener(HandleUpgradeClick);
            DisableUI();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            upgradeButton.onClick.RemoveListener(HandleUpgradeClick);
        }

        private void HandleUpgradeClick()
        {
            if (_skill == null) return;
            _skill.Level++;
            RefreshUI(_skill);
            Debug.Log(_skill.Level);
        }

        public void EnableFor(Skill skill)
        {
            _skill = skill;
            RefreshUI(skill);
            EnableUI();
        }

        private void RefreshUI(Skill skill)
        {
            skillSlot.EnableFor(skill);
            if(skill.SkillData.upgradeList.Count == 0) return;
            description.text = skill.SkillData.upgradeList[skill.Level].upgradeDescription;
            levelText.text = $"레벨 {skill.Level}";
        }

        public void Clear()
        {
            _skill = null;
            DisableUI(true);
        }
    }
}