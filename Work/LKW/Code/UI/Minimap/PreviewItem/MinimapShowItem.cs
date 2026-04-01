using Code.UI.Minimap.Core;
using UnityEngine;
using UnityEngine.UI;
using Work.LKW.Code.Items.ItemInfo;

namespace Code.UI.Minimap.PreviewItem
{
    public class MinimapShowItem : MinimapElement
    {
         private Image _image;

         protected override void Awake()
         {
             base.Awake();
             _image = GetComponent<Image>();
         }
    }
}