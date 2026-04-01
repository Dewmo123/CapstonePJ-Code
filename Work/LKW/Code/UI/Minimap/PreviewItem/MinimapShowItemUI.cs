using System.Collections.Generic;
using System.Linq;
using Chipmunk.GameEvents;
using Code.Events;
using UnityEngine;
using Sirenix.Utilities;
using Work.Code.GameEvents;
using Work.LKW.Code.Items.ItemInfo;

namespace Code.UI.Minimap.PreviewItem
{
    public class MinimapShowItemUI : MonoBehaviour
    {
        // 임시임
        [SerializeField] private MinimapUI minimapUI;
        // 임시임
        [SerializeField] private MinimapShowItem itemPrefab;
        
        private Dictionary<SpawnArea, MinimapItemSectionPanel> _sections;

        private void Awake()
        {
            _sections = GetComponentsInChildren<MinimapItemSectionPanel>()
                .ToDictionary(panel => panel.Area, panel => panel);
        }


        private void Start()
        {
            _sections.Values.ForEach(panel =>
            {
               
            });
        }

        private void OnDestroy()
        {
            _sections.Values.ForEach(panel =>
            {
                // RemoveMinimapElementEvent evt = new RemoveMinimapElementEvent(panel);
                // Bus.Raise(evt);
            });
        }

        private void OnEnable() => Bus.Subscribe<ShowItemsOnMap>(HandleShowItems);
        private void OnDisable() => Bus.Unsubscribe<ShowItemsOnMap>(HandleShowItems);

        private void HandleShowItems(ShowItemsOnMap evt)
        {
            // ClearAll();
            //
            // foreach (var item in evt.ItemList)
            // {
            //     var flags = item.spawnArea;
            //
            //     foreach (var pair in _sections)
            //     {
            //         var area = pair.Key;
            //
            //         if ((flags & area) == 0) continue;
            //
            //         var panel = pair.Value;
            //
            //         MinimapShowItem obj = Instantiate(itemPrefab);
            //         obj.SetItem(item);
            //         Vector2 initPos =  panel.AddPreviewItem(obj);
            //         
            //         obj.Rect.anchoredPosition = initPos;
            //         
            //         AddMinimapElementEvent addEvt = new AddMinimapElementEvent(obj);
            //         Bus.Raise(addEvt);
            //         
            //     }
            // }
            //
            // minimapUI.ToggleUI();
        }

        private void ClearAll()
        {
            foreach (var panel in _sections.Values)
            {
                panel.ClearPreviewItems(); 
            }
        }
    }
}