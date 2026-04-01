using Code.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.PlayerUI
{
    public class StaminaUI : UIBase
    {
        [SerializeField] private Image fill;

        public void SetFill(float amount)
        {
            fill.fillAmount = amount;
        }
    }
}