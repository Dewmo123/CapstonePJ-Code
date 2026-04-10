using System;
using Code.UI.Core;
using Code.UI.Core.Interaction;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Work.Code.UI.ContextMenu;
using Work.Code.UI.Core.Interaction;
using Work.LKW.Code.Items.ItemInfo;

namespace Work.Code.Crafting
{
    public class CraftItemUI : InteractableUI
    {
        [SerializeField] private ContextMenuSO craftItemMenu;
        [SerializeField] private Image icon;
        [SerializeField] private Image pin;
        [SerializeField] private Image background;
        [SerializeField] private Image outline;
        [SerializeField] private Image star;
        [SerializeField] private TextMeshProUGUI title;
        
        private Sequence _enableSeq;
        private float _duration = 0.3f;
        private readonly string _tooltipText = "우클릭으로 조합법 고정";

        [field: SerializeField] public Button ItemButton { get; set; }
        public CraftTreeSO Tree { get; private set; }
        public bool IsFavorite { get; set; }
        public bool IsPinned { get; set; }
        public event Action<CraftItemUI, bool> OnPinItem;
        public event Action<CraftTreeSO> OnRequestCraft;

        protected override void Awake()
        {
            base.Awake();
            BindTooltip(() => _tooltipText, 1f);
            BindContextMneu(craftItemMenu, () => this);
        }

        public void ToggleFavorite()
        {
            IsFavorite = !IsFavorite;
            star.gameObject.SetActive(IsFavorite);
        }

        public void Init(ItemDataSO item, bool isNotificate)
        {
            EnableUI();

            if (isNotificate)
                EnableTween();

            icon.sprite = item.itemImage;
            background.color = UIDefine.RarityColors[(int)item.rarity];
            title.text = item.itemName;
            star.gameObject.SetActive(IsFavorite);
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
            _enableSeq.Join(background.DOFade(1f, _duration).SetEase(Ease.OutCubic));
            _enableSeq.Join(icon.transform.DOScale(1f, _duration).SetEase(Ease.OutCubic));
            _enableSeq.Join(icon.DOFade(1f, _duration).SetEase(Ease.OutCubic));
            _enableSeq.Join(outline.DOFade(1f, _duration).SetEase(Ease.OutCubic));
            _enableSeq.SetAutoKill(true);
        }

        public void SetPin(bool isPinned)
        {
            IsPinned = isPinned;
            pin.gameObject.SetActive(isPinned);
        }

        public void TogglePin()
        {
            OnPinItem?.Invoke(this, !IsPinned);
        }

        public void RequestCraft()
        {
            OnRequestCraft?.Invoke(Tree);
        }

        public void Clear() { }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnbindTooltip();
            UnBindContextMneu();
        }
    }
}