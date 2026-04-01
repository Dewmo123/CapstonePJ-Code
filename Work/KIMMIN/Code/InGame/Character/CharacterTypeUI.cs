    using System;
    using Code.UI.Core;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    namespace Work.Code.Setting
    {
        public class CharacterTypeUI : MonoBehaviour
        {
            [SerializeField] private RectTransform rect;
            [SerializeField] private Image characterIcon;
            [SerializeField] private TextMeshProUGUI characterName;
            [SerializeField] private TextMeshProUGUI description;
            [SerializeField] private TextMeshProUGUI selectText;
            [SerializeField] private Canvas canvas;

            private UIEventHandler _uiEventHandler;
            private Vector2 _originPos;
            private Vector3 _originRot;
            //private readonly float _ySize = 50f;

            [field: SerializeField] public ChrarcterSO Character { get; private set; }

            [SerializeField] private Button _button;
            public event Action<CharacterTypeUI> OnSelect;
            public event Action<CharacterTypeUI> OnEnterHovering;
            public event Action OnExitHovering;
            
            private void Awake()
            {
                _uiEventHandler = UIUtility.GetOrAddComponent<UIEventHandler>(gameObject);
                _uiEventHandler.BindUIEvent(gameObject, HandlePointerEnter, EUIEvent.PointerEnter);
                _uiEventHandler.BindUIEvent(gameObject, HandlePointerExit, EUIEvent.PointerExit);
                _button.onClick.AddListener(HandleSelect);
                Debug.Assert(Character != null, "characterSO is null");
                
                characterName.text = Character.characterName;
                description.text = Character.description;
                characterIcon.sprite = Character.characterIcon;
                description.alpha = 0;
                selectText.alpha = 0;
                
                _originPos = rect.anchoredPosition;
                _originRot = rect.localEulerAngles;
            }

            private void OnDestroy()
            {
                UnbindPointerEvents();
                _button.onClick.RemoveListener(HandleSelect);
            }
            
            private void HandleSelect()
            {
                selectText.transform.DOScale(0, 0.5f).SetEase(Ease.InCirc);
                SetSize(1.06f, 0.2f, Ease.InOutQuad);
                OnSelect?.Invoke(this);
            }
            
            private void HandlePointerEnter(PointerEventData evt)
            {
                rect?.DOKill();
                description?.DOKill();
                characterIcon?.DOKill();
                selectText?.DOKill();
                
                description.DOFade(1f, 0.2f);
                selectText.DOFade(1f, 0.2f);
                characterIcon.DOColor(Color.white, 0.2f);

                ChardTween();
                OnEnterHovering?.Invoke(this);
            }
            
            private void HandlePointerExit(PointerEventData evt)
            {
                rect?.DOKill();
                description?.DOKill();
                characterIcon?.DOKill();
                selectText?.DOKill();
                
                rect.DOAnchorPos(_originPos, 0.25f);
                rect.DOLocalRotate(_originRot, 0.2f);
                description.DOFade(0f, 0.1f);
                selectText.DOFade(0f, 0.1f);
                characterIcon.DOColor(Color.black, 0.1f);
                OnExitHovering?.Invoke();
            }

            public void ChardTween()
            {
                rect.DOLocalRotate(Vector3.back * 5f,0.5f).SetEase(Ease.InOutSine)
                    .OnComplete(() => {
                        rect.DOLocalRotate(Vector3.forward * 5f, 1f)
                            .SetEase(Ease.InOutSine)
                            .SetLoops(-1, LoopType.Yoyo);
                    });

                rect.DOAnchorPosY(_originPos.y + 4f, 1f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            public void SetSize(float size, float duration, Ease ease)
            {
                rect.DOKill();
                rect.DOScale(size, duration).SetEase(ease);
            }

            public void SetSortOrder(int index)
            {
                canvas.sortingOrder = index;
            }

            public void UnbindPointerEvents()
            {
                _uiEventHandler.UnBindUIEvent(gameObject, HandlePointerEnter, EUIEvent.PointerEnter);
                _uiEventHandler.UnBindUIEvent(gameObject, HandlePointerExit, EUIEvent.PointerExit);
                _button.interactable = false;
            }
        }
    }