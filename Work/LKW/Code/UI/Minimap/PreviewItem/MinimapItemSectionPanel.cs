using System.Collections.Generic;
using Code.UI.Minimap.Core;
using NUnit.Framework;
using UnityEngine;
using Work.LKW.Code.Items.ItemInfo;

namespace Code.UI.Minimap.PreviewItem
{
    public class MinimapItemSectionPanel : MinimapElement
    {
        [field:SerializeField] public SpawnArea Area { get; set; }
        
        private List<MinimapShowItem> _items = new List<MinimapShowItem>();
        private List<RectTransform> _positions = new List<RectTransform>();
        
        private int _currentIndex = 0;

        protected override void Awake()
        {
            base.Awake();
            foreach (var child in transform)
            {
                _positions.Add(child as RectTransform);
            }
        }

        public Vector2 AddPreviewItem(MinimapShowItem showItem)
        {
            _items.Add(showItem);

            Vector3 worldPos = _positions[_currentIndex].transform.position;

            RectTransform mapTransform = transform.parent.GetComponent<RectTransform>(); 

            Vector3 localPos = mapTransform.InverseTransformPoint(worldPos);

            _currentIndex++;
            return (Vector2)localPos;
        }
        
        public void ClearPreviewItems()
        {
            _currentIndex = 0;
            _items.Clear();
        }
    }
}