using System;
using Code.Items.ItemInfo;
using Code.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI.NPC
{
    public class ItemSelectButton : IDisposable
    {
        private readonly Transform _root;
        private UnityAction _selectAction;
        private Action<PointerEventData> _deselectAction;

        protected readonly TextMeshProUGUI nameText;
        protected readonly Image icon;
        protected readonly Image background;
        protected readonly Image outlineImage;
        protected readonly Button selectBtn;
        protected readonly UIEventHandler eventHandler;
        protected ItemDataSO _itemData;
        private readonly Color _defaultOutlineColor;
        private static readonly Color32 SelectedOutlineColor = new(255, 220, 90, 255);

        public ItemDataSO ItemData => _itemData;
        public GameObject GameObject => _root.gameObject;

        protected ItemSelectButton(Transform root)
        {
            _root = root;
            selectBtn = root.GetComponent<Button>();
            eventHandler = root.GetComponent<UIEventHandler>();
            background = root.Find("background")?.GetComponent<Image>();
            icon = root.Find("background/Icon")?.GetComponent<Image>();
            nameText = root.Find("background/name background/name text")?.GetComponent<TextMeshProUGUI>();
            outlineImage = root.Find("outline")?.GetComponent<Image>();

            if (selectBtn == null || eventHandler == null || background == null || icon == null || nameText == null || outlineImage == null)
                throw new MissingReferenceException($"{nameof(ItemSelectButton)} setup is incomplete on {_root.name}.");

            _defaultOutlineColor = outlineImage.color;
        }

        protected void InitItem(ItemDataSO itemData)
        {
            if (itemData == null)
                throw new ArgumentNullException(nameof(itemData));

            _itemData = itemData;
            int rarityIndex = (int)_itemData.rarity;
            if ((uint)rarityIndex >= UIDefine.RarityColors.Length)
                throw new InvalidOperationException($"Unsupported rarity {_itemData.rarity} on {_itemData.itemName}.");

            background.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
            icon.enabled = true;
            nameText.transform.parent.gameObject.SetActive(true);
            nameText.gameObject.SetActive(true);

            background.color = UIDefine.RarityColors[rarityIndex];
            icon.sprite = itemData.itemImage;
            nameText.text = itemData.itemName;
            SetSelected(false);
            GameObject.SetActive(true);
        }

        protected void BindSelect(Action<ItemDataSO> onSelect)
        {
            if (_selectAction != null)
                selectBtn.onClick.RemoveListener(_selectAction);

            _selectAction = () => onSelect(_itemData);
            selectBtn.onClick.AddListener(_selectAction);
        }

        protected void BindDeselect(Action<ItemDataSO> onDeselect)
        {
            if (_deselectAction != null &&
                eventHandler.EventHandler.TryGetValue(EUIEvent.RightClick, out var rightClickAction))
            {
                rightClickAction -= _deselectAction;

                if (rightClickAction == null)
                    eventHandler.EventHandler.Remove(EUIEvent.RightClick);
                else
                    eventHandler.EventHandler[EUIEvent.RightClick] = rightClickAction;
            }

            _deselectAction = _ => onDeselect(_itemData);

            if (eventHandler.EventHandler.TryGetValue(EUIEvent.RightClick, out var boundAction))
                eventHandler.EventHandler[EUIEvent.RightClick] = boundAction + _deselectAction;
            else
                eventHandler.EventHandler[EUIEvent.RightClick] = _deselectAction;
        }

        protected void SetSelected(bool isSelected)
        {
            SetOutline(isSelected, isSelected);
        }

        protected void SetOutline(bool isVisible, bool isSelected)
        {
            outlineImage.gameObject.SetActive(isVisible);
            outlineImage.color = isSelected ? SelectedOutlineColor : _defaultOutlineColor;
        }

        public void Hide()
        {
            GameObject.SetActive(false);
        }

        public void Dispose()
        {
            if (_selectAction != null)
                selectBtn.onClick.RemoveListener(_selectAction);

            if (_deselectAction == null ||
                !eventHandler.EventHandler.TryGetValue(EUIEvent.RightClick, out var rightClickAction))
                return;

            rightClickAction -= _deselectAction;

            if (rightClickAction == null)
                eventHandler.EventHandler.Remove(EUIEvent.RightClick);
            else
                eventHandler.EventHandler[EUIEvent.RightClick] = rightClickAction;
        }
    }
}
