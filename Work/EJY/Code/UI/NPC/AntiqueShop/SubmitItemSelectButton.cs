using System;
using Code.Items.ItemInfo;
using TMPro;
using UnityEngine;

namespace Code.UI.NPC
{
    public class SubmitItemSelectButton : ItemSelectButton
    {
        private readonly GameObject _countRoot;
        private readonly TextMeshProUGUI _countText;
        private readonly GameObject _selectedCountRoot;
        private readonly TextMeshProUGUI _selectedCountText;

        public SubmitItemSelectButton(Transform root) : base(root)
        {
            _countRoot = root.Find("background/stack background")?.gameObject;
            _countText = root.Find("background/stack background/stack text")?.GetComponent<TextMeshProUGUI>();
            _selectedCountRoot = root.Find("background/SelectStackImage")?.gameObject;
            _selectedCountText = root.Find("background/SelectStackImage/SelectStackCenterImage/CntText")?.GetComponent<TextMeshProUGUI>();

            if (_countRoot == null || _countText == null || _selectedCountRoot == null || _selectedCountText == null)
                throw new MissingReferenceException($"{nameof(SubmitItemSelectButton)} setup is incomplete on {root.name}.");
        }

        public void Init(ItemDataSO itemData, int ownedCount, Action<ItemDataSO> onSelect, Action<ItemDataSO> onDeselect)
        {
            InitItem(itemData);
            BindSelect(onSelect);
            BindDeselect(onDeselect);
            SetOwnedCount(ownedCount);
            SetSelectedCount(0);
            SetSelectedState(false);
        }

        public void SetOwnedCount(int ownedCount)
        {
            _countRoot.SetActive(true);
            _countText.text = ownedCount.ToString();
        }

        public void SetSelectedState(bool isSelected)
        {
            SetOutline(true, isSelected);
        }

        public void SetSelectedCount(int selectedCount)
        {
            bool hasSelectedCount = selectedCount > 0;
            _selectedCountRoot.SetActive(hasSelectedCount);

            if (hasSelectedCount)
                _selectedCountText.text = selectedCount.ToString();
        }
    }
}
