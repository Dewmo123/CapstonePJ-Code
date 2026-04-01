using System;
using UnityEngine;

namespace Code.UI.Core
{
    public class LayoutUIBase : UIBase
    {
        public override void EnableUI(bool hasTween = false)
        {
            base.EnableUI(hasTween);
            gameObject.SetActive(true);
        }

        public override void DisableUI(bool hasTween = false)
        {
            base.DisableUI(hasTween);
            gameObject.SetActive(false);
        }
    }
}