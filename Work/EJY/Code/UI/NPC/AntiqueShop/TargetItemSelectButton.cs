using System;
using Code.Items.ItemInfo;
using UnityEngine;

namespace Code.UI.NPC
{
    public class TargetItemSelectButton : ItemSelectButton
    {
        private readonly GameObject _countRoot;
        private readonly GameObject _selectedCountRoot;

        public TargetItemSelectButton(Transform root) : base(root)
        {
            _countRoot = root.Find("background/stack background")?.gameObject;
            _selectedCountRoot = root.Find("background/SelectStackImage")?.gameObject;

            if (_countRoot == null || _selectedCountRoot == null)
                throw new MissingReferenceException($"{nameof(TargetItemSelectButton)} setup is incomplete on {root.name}.");
        }

        public void Init(ItemDataSO itemData, Action<ItemDataSO> onSelect)
        {
            InitItem(itemData);
            BindSelect(onSelect);
            _countRoot.SetActive(false);
            _selectedCountRoot.SetActive(false);
            SetOutline(true, false);
        }

        public void SetSelectedState(bool isSelected)
        {
            SetOutline(true, isSelected);
        }
    }
}
