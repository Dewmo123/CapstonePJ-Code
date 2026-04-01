using Chipmunk.GameEvents;
using Code.Hotbar;
using Code.InventorySystems.Items;
using Code.UI.Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.LKW.Code.Items;

namespace Code.InGame.Hotbar
{
    public class HotbarUI : MonoBehaviour, IUIElement<ItemSlot>
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI keyText;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Image icon;
        
        private Tween _hotbarTween;

        public int Index { get; private set; }

        private void Awake()
        {
            button.onClick.AddListener(OnPressed);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnPressed);
        }

        public void OnPressed()
        {
            _hotbarTween.Kill();
            transform.localScale = Vector3.one;
            _hotbarTween = transform.DOScale(0.9f, 0.07f)
                .SetLoops(2, LoopType.Yoyo);
            
            EventBus.Raise(new HotbarUseEvent(Index));
        }

        public void EnableFor(ItemSlot slot)
        {
            if(slot == null) return;

            if (slot.Item is EquipableItem equipableItem)
            {
                countText.gameObject.SetActive(true);
                icon.gameObject.SetActive(true);
                icon.sprite = equipableItem.ItemData.itemImage;
                countText.text = slot.Stack.ToString();
            }
        }

        public void Clear()
        {
            icon.gameObject.SetActive(false);
            countText.gameObject.SetActive(false);
        }
        
        private void SetIndexText()
        {
            if (keyText != null)
            {
                keyText.text = Index.ToString();
            }
        }

        public void SetIndex(int idx)
        {
            Index = idx;
            SetIndexText();
        }
    }
}