using System;
using UnityEngine;

namespace Code.UI.Core
{
    public class UIPanel : UIBase
    {
        public override EUILayer Layer => EUILayer.Panel;

        protected override void Awake()
        {
            base.Awake();
            DisableUI();
        }
    }
}