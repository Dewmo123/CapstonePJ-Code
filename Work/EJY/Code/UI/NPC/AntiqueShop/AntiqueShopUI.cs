using System.Collections.Generic;
using System.Linq;
using Chipmunk.ComponentContainers;
using Code.Items;
using Code.Items.ItemInfo;
using Code.Players;
using Scripts.Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.NPC
{
    public class AntiqueShopUI : NPCInteractUIContent
    {
        [SerializeField] private int exchangeCnt;
        [SerializeField] private Button exchangeBtn;
        [SerializeField] private TextMeshProUGUI exchangeCountText;
        [SerializeField] private Transform targetItemGridTrm;
        [SerializeField] private Transform submitItemGridTrm;
        [SerializeField] private ItemDataBaseSO itemDB;

        private readonly Dictionary<ItemDataSO, int> _selectedSubmitItems = new();
        private readonly List<TargetItemSelectButton> _targetItemSelectButtons = new();
        private readonly List<SubmitItemSelectButton> _submitItemSelectButtons = new();
        private Player _player;
        private PlayerInventory _playerInventory;
        private ItemDataSO _targetItemData;
        private RectTransform _targetItemContentRect;
        private RectTransform _submitItemContentRect;
        private GameObject _targetItemTemplate;
        private GameObject _submitItemTemplate;

        public override void Init(Player player)
        {
            base.Init(player);
            _player = player;

            exchangeBtn.onClick.AddListener(HandleExchangeBtnClick);

            if (_targetItemContentRect == null)
                SetupTargetItemGrid();

            if (_submitItemContentRect == null)
                SetupSubmitItemGrid();

            RefreshTargetButtons();
            UpdateExchangeState();
        }

        public override void EnableUI(bool isFade = false)
        {
            if (_playerInventory == null)
            {
                _playerInventory = _player.Get<PlayerInventory>();
                if (_playerInventory == null)
                    throw new MissingReferenceException($"{nameof(AntiqueShopUI)} requires {nameof(PlayerInventory)}.");

                _playerInventory.InventoryChanged -= HandleInventoryChanged;
                _playerInventory.InventoryChanged += HandleInventoryChanged;
            }

            _selectedSubmitItems.Clear();
            RefreshSubmitButtons();
            UpdateExchangeState();
            base.EnableUI(isFade);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            exchangeBtn.onClick.RemoveAllListeners();

            if (_playerInventory != null)
                _playerInventory.InventoryChanged -= HandleInventoryChanged;

            foreach (var targetButton in _targetItemSelectButtons)
            {
                targetButton.Dispose();
            }

            foreach (var submitButton in _submitItemSelectButtons)
            {
                submitButton.Dispose();
            }
        }

        private void HandleExchangeBtnClick()
        {
            if (!exchangeBtn.interactable)
                return;

            foreach (var pair in _selectedSubmitItems)
            {
                if (_playerInventory.RemoveItemByData(pair.Key, pair.Value) == false)
                    throw new UnityException($"Failed to remove submit item : {pair.Key.itemName}");
            }

            if (_playerInventory.TryAddItem(_targetItemData.CreateItem().Item) == false)
                throw new UnityException($"Failed to add target item : {_targetItemData.itemName}");

            _selectedSubmitItems.Clear();
            RefreshSubmitButtons();
            UpdateExchangeState();
        }

        private void SelectSubmitItem(ItemDataSO itemData)
        {
            if (GetSelectedSubmitItemCount() == exchangeCnt)
                return;

            int currentSelectedCount = GetSelectedCount(itemData);
            if (currentSelectedCount == _playerInventory.GetItemCount(itemData))
                return;

            _selectedSubmitItems[itemData] = currentSelectedCount + 1;

            UpdateSubmitButtonStates();
            UpdateExchangeState();
        }

        private void DeselectSubmitItem(ItemDataSO itemData)
        {
            int currentSelectedCount = GetSelectedCount(itemData);
            if (currentSelectedCount == 0)
                return;

            if (currentSelectedCount == 1)
                _selectedSubmitItems.Remove(itemData);
            else
                _selectedSubmitItems[itemData] = currentSelectedCount - 1;

            UpdateSubmitButtonStates();
            UpdateExchangeState();
        }

        private void SelectTargetItem(ItemDataSO itemData)
        {
            _targetItemData = itemData;

            foreach (var targetButton in _targetItemSelectButtons)
            {
                if (targetButton.GameObject.activeSelf == false)
                    continue;

                targetButton.SetSelectedState(targetButton.ItemData == itemData);
            }

            UpdateExchangeState();
        }

        private void RefreshTargetButtons()
        {
            List<ItemDataSO> targetItems = itemDB.GetItemsByType(ItemType.Material)
                .Where(item => item.rarity == Rarity.Common)
                .ToList();
            EnsureTargetButtonCount(targetItems.Count);

            foreach (var targetButton in _targetItemSelectButtons)
            {
                targetButton.Hide();
            }

            for (int i = 0; i < targetItems.Count; i++)
            {
                _targetItemSelectButtons[i].Init(targetItems[i], SelectTargetItem);
                _targetItemSelectButtons[i].SetSelectedState(targetItems[i] == _targetItemData);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_targetItemContentRect);
        }

        private void RefreshSubmitButtons()
        {
            foreach (ItemDataSO itemData in _selectedSubmitItems.Keys.ToList())
            {
                int remainCount = _playerInventory.GetItemCount(itemData);
                if (remainCount == 0)
                {
                    _selectedSubmitItems.Remove(itemData);
                    continue;
                }

                if (_selectedSubmitItems[itemData] > remainCount)
                    _selectedSubmitItems[itemData] = remainCount;
            }

            List<MaterialItem> submitItems = _playerInventory.GetItems<MaterialItem>();
            EnsureSubmitButtonCount(submitItems.Count);

            foreach (var submitButton in _submitItemSelectButtons)
            {
                submitButton.Hide();
            }

            for (int i = 0; i < submitItems.Count; i++)
            {
                ItemDataSO itemData = submitItems[i].ItemData;
                _submitItemSelectButtons[i].Init(itemData, _playerInventory.GetItemCount(itemData), SelectSubmitItem, DeselectSubmitItem);
                _submitItemSelectButtons[i].SetSelectedState(GetSelectedCount(itemData) > 0);
                _submitItemSelectButtons[i].SetSelectedCount(GetSelectedCount(itemData));
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_submitItemContentRect);
        }

        private void HandleInventoryChanged()
        {
            RefreshSubmitButtons();
            UpdateExchangeState();
        }

        private void UpdateSubmitButtonStates()
        {
            foreach (var submitButton in _submitItemSelectButtons)
            {
                if (submitButton.GameObject.activeSelf == false)
                    continue;

                submitButton.SetOwnedCount(_playerInventory.GetItemCount(submitButton.ItemData));
                submitButton.SetSelectedState(GetSelectedCount(submitButton.ItemData) > 0);
                submitButton.SetSelectedCount(GetSelectedCount(submitButton.ItemData));
            }
        }

        private int GetSelectedCount(ItemDataSO itemData)
        {
            if (_selectedSubmitItems.TryGetValue(itemData, out int count))
                return count;

            return 0;
        }

        private int GetSelectedSubmitItemCount()
        {
            return _selectedSubmitItems.Values.Sum();
        }

        private void UpdateExchangeState()
        {
            int selectedSubmitItemCount = GetSelectedSubmitItemCount();
            exchangeCountText.text = $"{selectedSubmitItemCount} / {exchangeCnt}";
            exchangeBtn.interactable = _targetItemData != null && selectedSubmitItemCount == exchangeCnt;
        }

        private void SetupTargetItemGrid()
        {
            _targetItemContentRect = SetupScrollableGrid(targetItemGridTrm);
            _targetItemTemplate = CreateButtonTemplate(_targetItemContentRect);

            for (int i = 0; i < _targetItemContentRect.childCount; i++)
            {
                Transform child = _targetItemContentRect.GetChild(i);
                if (child.gameObject == _targetItemTemplate)
                    continue;

                _targetItemSelectButtons.Add(new TargetItemSelectButton(child));
            }
        }

        private void SetupSubmitItemGrid()
        {
            _submitItemContentRect = SetupScrollableGrid(submitItemGridTrm);
            _submitItemTemplate = CreateButtonTemplate(_submitItemContentRect);

            for (int i = 0; i < _submitItemContentRect.childCount; i++)
            {
                Transform child = _submitItemContentRect.GetChild(i);
                if (child.gameObject == _submitItemTemplate)
                    continue;

                _submitItemSelectButtons.Add(new SubmitItemSelectButton(child));
            }
        }

        private void EnsureTargetButtonCount(int requiredCount)
        {
            while (_targetItemSelectButtons.Count < requiredCount)
            {
                GameObject buttonObject = Instantiate(_targetItemTemplate, _targetItemContentRect);
                buttonObject.name = _targetItemTemplate.name.Replace(" Template", string.Empty);
                buttonObject.SetActive(true);
                _targetItemSelectButtons.Add(new TargetItemSelectButton(buttonObject.transform));
            }
        }

        private void EnsureSubmitButtonCount(int requiredCount)
        {
            while (_submitItemSelectButtons.Count < requiredCount)
            {
                GameObject buttonObject = Instantiate(_submitItemTemplate, _submitItemContentRect);
                buttonObject.name = _submitItemTemplate.name.Replace(" Template", string.Empty);
                buttonObject.SetActive(true);
                _submitItemSelectButtons.Add(new SubmitItemSelectButton(buttonObject.transform));
            }
        }

        private RectTransform SetupScrollableGrid(Transform root)
        {
            GridLayoutGroup gridLayout = root.GetComponent<GridLayoutGroup>();
            if (gridLayout == null)
                throw new MissingComponentException($"{root.name} requires {nameof(GridLayoutGroup)}.");

            RectTransform viewportRect = root as RectTransform;
            if (viewportRect == null)
                throw new MissingComponentException($"{root.name} requires {nameof(RectTransform)}.");

            if (root.childCount == 0)
                throw new UnityException($"{root.name} requires at least one item button.");

            RectTransform contentRect = new GameObject("Content", typeof(RectTransform)).GetComponent<RectTransform>();
            contentRect.SetParent(root, false);
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = Vector2.zero;

            GridLayoutGroup contentGrid = contentRect.gameObject.AddComponent<GridLayoutGroup>();
            contentGrid.padding = gridLayout.padding;
            contentGrid.cellSize = gridLayout.cellSize;
            contentGrid.spacing = gridLayout.spacing;
            contentGrid.startCorner = gridLayout.startCorner;
            contentGrid.startAxis = gridLayout.startAxis;
            contentGrid.childAlignment = gridLayout.childAlignment;
            contentGrid.constraint = gridLayout.constraint;
            contentGrid.constraintCount = gridLayout.constraintCount;

            ContentSizeFitter contentSizeFitter = contentRect.gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            if (root.GetComponent<RectMask2D>() == null)
                root.gameObject.AddComponent<RectMask2D>();

            ScrollRect scrollRect = root.GetComponent<ScrollRect>();
            if (scrollRect == null)
                scrollRect = root.gameObject.AddComponent<ScrollRect>();

            scrollRect.content = contentRect;
            scrollRect.viewport = viewportRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 40f;

            int itemButtonCount = root.childCount - 1;
            for (int i = 0; i < itemButtonCount; i++)
            {
                root.GetChild(0).SetParent(contentRect, false);
            }

            gridLayout.enabled = false;
            return contentRect;
        }

        private GameObject CreateButtonTemplate(RectTransform contentRect)
        {
            if (contentRect.childCount == 0)
                throw new UnityException($"{contentRect.name} requires at least one item button.");

            GameObject template = Instantiate(contentRect.GetChild(0).gameObject, contentRect);
            template.name = $"{template.name} Template";
            template.SetActive(false);
            return template;
        }
    }
}
