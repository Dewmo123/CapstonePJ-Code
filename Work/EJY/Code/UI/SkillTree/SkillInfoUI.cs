using System;
using Chipmunk.GameEvents;
using Code.GameEvents;
using Code.SkillSystem.Upgrade;
using Scripts.SkillSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.SkillTree
{
    public class SkillInfoUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI skillName;
        [SerializeField] private TextMeshProUGUI skillDescription;
        [SerializeField] private CanvasGroup canvasGroup;

        private Skill _targetSkill;
        private SkillUpgradeSO _upgradeData;

        private void Awake()
        {
            Bus.Subscribe<SkillInfoUIUpdateEvent>(HandleSkillInfoUIUpdate);
            UIAlpha(0);
        }

        private void OnDestroy()
        {
            Bus.Unsubscribe<SkillInfoUIUpdateEvent>(HandleSkillInfoUIUpdate);
        }

        private void HandleSkillInfoUIUpdate(SkillInfoUIUpdateEvent evt)
        {
            _targetSkill = evt.targetSkill;
            _upgradeData = evt.upgradeData;
            
            Debug.Assert(_targetSkill != null && _upgradeData != null, "event is invalid");

            UIAlpha(1);
            icon.sprite = _upgradeData.upgradeIcon;
            skillName.SetText(_upgradeData.UpgradeTitle);
            skillDescription.SetText(_upgradeData.upgradeDescription);
        }

        public void UpgradeSkill()
        {
            if(_targetSkill == null || _upgradeData == null) return;
            
            Bus.Raise( new SkillUpgradeEvent(_targetSkill, _upgradeData));
        }

        private void UIAlpha(float value)
        {
            canvasGroup.alpha = value;
        }
    }
}