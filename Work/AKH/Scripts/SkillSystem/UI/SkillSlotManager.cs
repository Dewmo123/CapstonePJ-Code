using Scripts.SkillSystem.Manage;
using AYellowpaper.SerializedCollections;
using Chipmunk.GameEvents;
using Scripts.SkillSystem.SkillEvents;
using System;
using UnityEngine;

namespace Scripts.SkillSystem.UI
{
    public class SkillSlotManager : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<ActiveSlotType, SkillSlotUI> skillSlots;
        private void Awake()
        {
            EventBus.Subscribe<SkillCooldownEvent>(HandleSkillCooldown);
        }
        private void OnDestroy()
        {
            EventBus.Unsubscribe<SkillCooldownEvent>(HandleSkillCooldown);
        }
        private void HandleSkillCooldown(SkillCooldownEvent evt)
        {
            if(skillSlots.TryGetValue(evt.slotType,out SkillSlotUI ui))
            {
                ui.InitSlot(evt.skillData, evt.total);
            }
        }
    }
}
