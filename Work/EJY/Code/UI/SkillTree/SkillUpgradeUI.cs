using System;
using Chipmunk.GameEvents;
using Code.GameEvents;
using Code.SkillSystem.Upgrade;
using Code.UI.Core;
using DG.Tweening;
using Scripts.SkillSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Work.Code.Setting;

namespace Code.UI.SkillTree
{
    public class SkillUpgradeUI : MonoBehaviour
    {
        [field: SerializeField] public Button upgradeButton { get; private set; }
        [field: SerializeField] public SkillUpgradeSO UpgradeData { get; private set; }
        
        [SerializeField] private Image upgradeImage;
        [SerializeField] private Image outline;
        [SerializeField] private TextMeshProUGUI upgradeText;
        [SerializeField] private Image lockImage;
        
        public Skill TargetSkill { get; private set; }
        [field: SerializeField] public RectTransform RectTrm { get; private set; }

        private UIEventHandler _eventHandler;
        private ChrarcterSO _character;

        private void Awake()
        {
            _eventHandler = UIUtility.GetOrAddComponent<UIEventHandler>(gameObject);
            _eventHandler.BindUIEvent(gameObject, HandlePointEnter, EUIEvent.PointerEnter);
            _eventHandler.BindUIEvent(gameObject, HandlePointExit, EUIEvent.PointerExit);
            
            EventBus.Subscribe<SkillUpgradeEvent>(HandleUpgrade);
        }

        private void HandleUpgrade(SkillUpgradeEvent evt)
        {
            if (evt.upgradeData == UpgradeData)
            {
                Purchase();
            }
        }

        private void OnDestroy()
        {
            _eventHandler.UnBindUIEvent(gameObject, HandlePointEnter, EUIEvent.PointerEnter);
            _eventHandler.UnBindUIEvent(gameObject, HandlePointExit, EUIEvent.PointerExit);
            
            EventBus.Unsubscribe<SkillUpgradeEvent>(HandleUpgrade);
        }

        private void HandlePointEnter(PointerEventData evt)
        {
            gameObject.transform.DOKill();
            gameObject.transform.DOScale(1.2f, 0.25f)
                .SetEase(Ease.OutBack).SetUpdate(true);
        }

        private void HandlePointExit(PointerEventData evt)
        {
            gameObject.transform.DOKill();
            gameObject.transform.DOScale(1f, 0.25f)
                .SetEase(Ease.OutBack).SetUpdate(true);
        }
        
        public void SetCharacter(ChrarcterSO character) => _character = character;
        public void SetTargetSkill(Skill skill) => TargetSkill = skill;
        public void SetUnlock(bool isUnlock) => lockImage.gameObject.SetActive(!isUnlock);

        public void Purchase()
        {
            upgradeImage.DOColor(_character.characterColor, 0.6f)
                .SetUpdate(true);
            outline.DOColor(_character.characterColor, 0.6f)
                .SetUpdate(true);
        }
        
        public void UpdateUpgradeText(int count)
        {
            if(upgradeText == null) return;
            
            if(UpgradeData.maxUpgradeCnt > 1)
                upgradeText.text = $"{count}/{UpgradeData.maxUpgradeCnt}";
            else
                upgradeText.text = string.Empty;
        }

        private void OnValidate()
        {
            if (UpgradeData == null) return;
            gameObject.name = $"SkillUpgradeBtn_{UpgradeData.UpgradeTitle}";
            
            if(upgradeImage != null)
                upgradeImage.sprite = UpgradeData.upgradeIcon;
            
            UpdateUpgradeText(0);
        }
    }
}