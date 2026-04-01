using Code.UI.Minimap.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.UI.Minimap.SectionName
{
    public class SectionNameText : MinimapElement
    {
        public TextMeshProUGUI NameText { get; set; }

        protected override void Awake()
        {
            base.Awake();
            NameText = GetComponent<TextMeshProUGUI>();
        }
    }
}