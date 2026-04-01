using System;
using Code.SkillSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.SkillSystem.UI
{
    public class SkillSlotUI : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Image fill;
        [SerializeField] private TextMeshProUGUI cooldownText;

        private float _totalTime;
        private float _currentTime;
        
        public void InitSlot(SkillDataSO skillData, float total)
        {
            image.sprite = skillData?.skillIcon;
            _totalTime = total;
            _currentTime = total;
        }

        public void OnSkillUsed()
        {
            _currentTime = _totalTime;
        }

        private void Update()
        {
            if (_currentTime <= 0) return;
            _currentTime -= Time.deltaTime;
            fill.fillAmount = 1 - (_currentTime / _totalTime);
            cooldownText.text = $"{_currentTime.ToString("F1")}s";
        }
    }
}
