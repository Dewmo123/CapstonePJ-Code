using System;
using Code.UI.Core;
using Code.UI.Core.Interaction;
using Code.UI.Popup;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Work.Code.UI.Core.Interaction;
using Work.LKW.Code.Items.ItemInfo;

namespace Work.Code.Crafting
{
    public class CraftItemUI : InteractableUI, IUIElement<ItemDataSO>, IPopupable
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image pin;
        [SerializeField] private Image background;
        [SerializeField] private Image outline;
        [SerializeField] private Image star;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Button favoriteButton;
        
        private Color _favoriteColor = new Color(1f, 0.8f, 0.25f);
        private Color _unFavoriteColor = new Color(0.1f, 0.1f, 0.1f);
        private Sequence _enableSeq;
        private float _duration = 0.3f;
        private readonly string _tooltipText = "우클릭으로 조합법 고정";

        [field: SerializeField] public Button ItemButton { get; set; }
        public CraftTreeSO Tree { get; private set; }
        public bool IsFavorite { get; set; }
        public event Action<CraftItemUI, bool> OnRightClick;
        public event Action<Func<object>, ICallbackData> OnClickHandler;
        
        private readonly ConfirmCallback _callback = new();


        protected override void Awake()
        {
            base.Awake();
            favoriteButton.onClick.AddListener(HandleFavoriteClicked);
            BindTooltip(gameObject, () => _tooltipText, 1f);
            BindPopup(this);
        }

        private void HandleFavoriteClicked()
        {
            IsFavorite = !IsFavorite;
            star.color = IsFavorite ? _favoriteColor : _unFavoriteColor;
        }

        public void EnableFor(ItemDataSO item)
        {
            gameObject.SetActive(true);
            EnableTween();

            icon.sprite = item.itemImage;
            background.color = UIDefine.RarityColors[(int)item.rarity];
            title.text = item.itemName;
            star.color = IsFavorite ? _favoriteColor : _unFavoriteColor;
        }
        
        public void SetTree(CraftTreeSO tree) => Tree = tree;

        private void EnableTween()
        {
            background.DOKill();
            icon.DOKill();
            outline.DOKill();
            _enableSeq?.Kill();

            background.transform.localScale = Vector3.one * 0.925f;
            icon.transform.localScale = Vector3.one * 0.85f;
            outline.color = new Color(outline.color.r, outline.color.g, outline.color.b, 0f);
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0f);
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);

            _enableSeq = DOTween.Sequence();
            _enableSeq.Join(background.transform.DOScale(1f, _duration).SetEase(Ease.OutCubic));
            _enableSeq.Join(icon.transform.DOScale(1f, _duration).SetEase(Ease.OutCubic));
            _enableSeq.Join(outline.DOFade(1f, _duration).SetEase(Ease.OutCubic));
            _enableSeq.Join(background.DOFade(1f, _duration).SetEase(Ease.OutCubic));
            _enableSeq.Join(icon.DOFade(1f, _duration).SetEase(Ease.OutCubic));
            _enableSeq.SetAutoKill(true);
        }

        public void OnClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                bool pinStatus = !pin.gameObject.activeSelf;
                OnRightClick?.Invoke(this, pinStatus);
                OnClickHandler?.Invoke(() => "진짜로 하시겠어요?", _callback);
            }
        }

        public void SetPin(bool isPinned)
        {
            pin.gameObject.SetActive(isPinned);
        }

        public void Clear()
        {
            gameObject.SetActive(false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnbindTooltip(gameObject);
        }
    }
}