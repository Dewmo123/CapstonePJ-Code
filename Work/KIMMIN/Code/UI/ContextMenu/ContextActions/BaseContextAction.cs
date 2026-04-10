using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.Code.UI.Core.Interaction;

namespace Work.Code.UI.ContextMenu
{
    public class BaseContextAction : InteractableUI { }
    
    [RequireComponent(typeof(Button))]
    public abstract class BaseContextAction<T> : BaseContextAction, IContextAction<T>
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Image icon;
        private Button _contextButton;
        private T _data;
        
        [field: SerializeField] public ContextActionSO ContextActionSO { get; private set; }
        protected virtual string HelpText => CheckCondition(_data) ? ActiveText : InactiveText;
        protected abstract string ActiveText { get; }
        protected abstract string InactiveText { get; }
        public event Action<T> ContextAction;
        public event Action OnCallbackInvoked;

        protected override void Awake()
        {
            base.Awake();
            _contextButton = GetComponent<Button>();
        }

        private void OnDisable()
        {
            UnbindTooltip();
        }

        public override void DisableUI(bool isFade = false)
        {
            base.DisableUI(isFade);
            gameObject.SetActive(false);
        }

        public override void EnableUI(bool isFade = false)
        {
            base.EnableUI(isFade);
            gameObject.SetActive(true);
        }

        public void Init(T data)
        {
            _data = data;

            EnableUI();
            ResetEvents();
            InitAction(data);
            BindTooltip(() => HelpText, 0.5f);
            
            title.text = HelpText;
            icon.sprite = ContextActionSO.actionIcon;
            name = $"{typeof(T).Name}ContextAction";
        }

        public void ResetEvents()
        {
            UnbindTooltip();
            ContextAction = null;
            OnCallbackInvoked = null;
        }

        private void InitAction(T data)
        {
            if(_contextButton == null) 
                _contextButton = GetComponent<Button>();
            
            _contextButton.onClick.RemoveAllListeners();
            _contextButton.onClick.AddListener(() =>
            {
                OnCallbackInvoked?.Invoke();
                ContextAction?.Invoke(data);
            });
            
            ContextAction += OnAction;
        }
        
        public abstract bool CheckCondition(T data);
        public abstract void OnAction(T data);
        public virtual bool CanShow(T data) => true;
    }
}