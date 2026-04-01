using System;
using Chipmunk.GameEvents;
using Code.UI.Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Work.Code.GameEvents;
using Work.Code.UI.Core.Interaction;

namespace Work.Code.Crafting
{
    public class CraftNodeUI : InteractableUI, IUIElement<NodeData, int, bool>
    {
        [SerializeField] private Image icon; 
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Image background;
        [SerializeField] private Image outline;

        private string _tooltipText = "클릭해 하위 트리로 이동";
        private float _duration = 0.4f;
        private Sequence _enableSeq;
        
        [field: SerializeField] public RectTransform LineStartRect { get; set; }
        [field: SerializeField] public RectTransform LineEndRect { get; set; }
        [field: SerializeField] public Button NodeButton { get; set; }
        
        public RectTransform Rect => icon.transform as RectTransform;
        public NodeData Data { get; set; }

        public void EnableFor(NodeData data, int count = 1, bool isNeedItem = true)
        {
            if (background == null) return; 
            gameObject.SetActive(true);
            background.color = UIDefine.RarityColors[(int)data.Item.rarity];
            icon.sprite = data.Item.itemImage;
            if (isNeedItem)
            {
                countText.text = $"{data.Count}개";
                countText.color = Color.white;
            }
            else
            {
                countText.text = $"{count}/{data.Count}";
                countText.color = count >= data.Count ? Color.white : Color.red;
            }

            Data = data;

            SubscribeEvents();
            EnableTween();
        }

        private void SubscribeEvents()
        {
            EventBus.Raise(new UnBindTooltipEvent(gameObject));
            
            _eventHandler.BindUIEvent(gameObject, _ => gameObject.transform
                .DOScale(1.2f, 0.3f).SetEase(Ease.OutBack), EUIEvent.PointerEnter);
            _eventHandler.BindUIEvent(gameObject, _ => gameObject.transform
                .DOScale(1f, 0.3f).SetEase(Ease.OutBack), EUIEvent.PointerExit);
            
            BindTooltip(gameObject, () => Data.Item);
        }
        
        private void EnableTween()
        {
            _enableSeq?.Kill();

            background.transform.localScale = Vector3.one * 0.85f;
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

        public void Clear()
        { 
            gameObject.SetActive(false);
            NodeButton?.onClick.RemoveAllListeners();
            UnbindTooltip(gameObject);
        }

        public void SubscribeClick(UnityAction action)
        {
            NodeButton.onClick.AddListener(action);
        }

        public void SubscribeTooltip()
        {
            BindTooltip(gameObject, () => _tooltipText);
        }
    }
}