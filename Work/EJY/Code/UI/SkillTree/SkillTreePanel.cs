/*using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Chipmunk.GameEvents;
using Code.GameEvents;
using Code.SkillSystem.Upgrade;
using Code.UI.Core;
using DewmoLib.Dependencies;
using InGame.PlayerUI;
using Scripts.SkillSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Work.Code.Core;
using Work.Code.SkillTree;

namespace Code.UI.SkillTree
{
    public class SkillTreePanel : UIPanel
    {
        [SerializeField] SerializedDictionary<CharacterType, GameObject> _skillTrees;
        [SerializeField] private GameObject content;
        [SerializeField] private TextMeshProUGUI characterText;
        [SerializeField] private PlayerInputSO playerInput;
        private Dictionary<SkillUpgradeSO, SkillUpgradeUI> _upgradeButtonDict = new();
        private Dictionary<SkillUpgradeSO, UILineRenderer> _lineRenderers = new();

        [Inject] private CharacterContainer _characterContainer;

        [field: SerializeField] public RectTransform LineParentTrm { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Bus.Subscribe<SkillTreeUpdateEvent>(HandleSkillTreeUpdate);
            playerInput.OnSkillTreePressed += ToggleUI;

            _upgradeButtonDict = GetComponentsInChildren<SkillUpgradeUI>(true)
                .ToDictionary(btnUI => btnUI.UpgradeData);

            foreach (var ui in _upgradeButtonDict.Values)
            {
                ui.SetCharacter(_characterContainer.Character);
            }

            _lineRenderers = LineParentTrm.GetComponentsInChildren<UILineRenderer>().ToDictionary(line => line.data);

            foreach (var skilltree in _skillTrees.Values)
            {
                skilltree.gameObject.SetActive(false);
            }

            characterText.text = _characterContainer.Character.characterName;
            _skillTrees[_characterContainer.Character.characterType].gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            Bus.Unsubscribe<SkillTreeUpdateEvent>(HandleSkillTreeUpdate);
            playerInput.OnSkillTreePressed -= ToggleUI;
        }

        private void HandleSkillTreeUpdate(SkillTreeUpdateEvent evt)
        {
            foreach (var skill in evt.skillDict.Values)
            {
                if (skill.SkillData == null) continue;

                foreach (var upgradeData in skill.SkillData.upgradeList)
                {

                    if (_upgradeButtonDict.TryGetValue(upgradeData, out SkillUpgradeUI btn))
                    {
                        //int cnt = skill.GetUpgradeCount(upgradeData);

                        if (_lineRenderers.TryGetValue(upgradeData, out UILineRenderer lineRenderer))
                        {
                            //if (skill.CanUpgradeSkill(upgradeData))
                            //{
                            //    lineRenderer.lineColor = Color.green;
                            //    lineRenderer.transform.SetParent(null);
                            //    lineRenderer.transform.SetParent(LineParentTrm);
                            //    lineRenderer.SetVerticesDirty();
                            //}
                            //else
                            //{
                            //    lineRenderer.lineColor = Color.white;
                            //    lineRenderer.SetVerticesDirty();
                            //}
                        }

                        //btn.SetUnlock(cnt > 0);
                        //btn.UpdateUpgradeText(cnt);
                        //btn.SetTargetSkill(skill);
                    }
                }
            }
        }

        public override void ToggleUI(bool hasTween = false)
        {
            base.ToggleUI();
            playerInput.SetPlayerInput(!IsActive);
            Time.timeScale = IsActive ? 0 : 1;
        }


        public override void EnableUI(bool hasTween = false)
        {
            base.EnableUI();
            
            foreach (var ui in _upgradeButtonDict.Values)
            {
                ui.upgradeButton.onClick.AddListener(() =>
                {
                    if (ui.UpgradeData == null) return;
                    Bus.Raise(new SkillInfoUIUpdateEvent(ui.TargetSkill, ui.UpgradeData));
                });
            }
        }

        public override void DisableUI(bool hasTween = false)
        {
            base.DisableUI();
            
            foreach (var ui in _upgradeButtonDict.Values)
            {
                ui.upgradeButton.onClick?.RemoveAllListeners();
            }
        }
    }
}*/