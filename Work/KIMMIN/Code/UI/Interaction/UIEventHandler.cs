using System;
using System.Collections.Generic;
using Code.UI.Core.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;
using Work.Code.UI.Interaction;

namespace Code.UI.Core
{
    public enum EUIEvent
    {
        PointerEnter,
        PointerExit,
        PointerClick,
        PointerDown,
        PointerUp,
        DragBegin,
        Drag,
        DragEnd,
        Drop,
        None
    }
    
    [DefaultExecutionOrder(-15)]
    public class UIEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        public Dictionary<EUIEvent, Action<PointerEventData>> EventHandler { get; private set; }

        private void Awake()
        {
            EventHandler = new();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(EventHandler.TryGetValue(EUIEvent.PointerEnter, out var evt))
                evt?.Invoke(eventData);
            
            if(TryGetComponent<IHoverable>(out var draggable))
                draggable.OnHoverEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(EventHandler.TryGetValue(EUIEvent.PointerExit, out var evt))
                evt?.Invoke(eventData);
            
            if(TryGetComponent<IHoverable>(out var draggable))
                draggable.OnHoverExit(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(EventHandler.TryGetValue(EUIEvent.PointerClick, out var evt))
                evt?.Invoke(eventData);

            if (TryGetComponent<IClickable>(out var clickable))
                clickable.OnClick(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(EventHandler.TryGetValue(EUIEvent.PointerDown, out var evt))
                evt?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(EventHandler.TryGetValue(EUIEvent.PointerUp, out var evt))
                evt?.Invoke(eventData);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {            
            if(EventHandler.TryGetValue(EUIEvent.DragBegin, out var evt)) 
                evt?.Invoke(eventData);
            
            if(TryGetComponent<IDraggable>(out var draggable))
                draggable.OnDragStart(eventData);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if(EventHandler.TryGetValue(EUIEvent.Drag, out var evt))
                evt?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(EventHandler.TryGetValue(EUIEvent.DragEnd, out var evt))
                evt?.Invoke(eventData);
            
            if(TryGetComponent<IDraggable>(out var draggable))
                draggable.OnDragEnd(eventData);
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            if(EventHandler.TryGetValue(EUIEvent.Drop, out var evt))
                evt?.Invoke(eventData);
            
            if(TryGetComponent<IDroppable>(out var droppable))
                droppable.OnDrop(eventData);
        }
        
        /// <summary>
        /// UIHandler 이벤트 연결 함수
        /// </summary>
        /// <param name="go">연결 대상 UI</param>
        /// <param name="action">콜백</param>
        /// <param name="type">핸들러 이벤트 타입</param>
        public void BindUIEvent(GameObject go, Action<PointerEventData> action, EUIEvent type = EUIEvent.PointerClick)
        {
            UIEventHandler evt = UIUtility.GetOrAddComponent<UIEventHandler>(go);

            if (evt.EventHandler.ContainsKey(type))
            {
                evt.EventHandler[type] -= action;
                evt.EventHandler[type] += action;
            }
            else if (!evt.EventHandler.ContainsKey(type))
                evt.EventHandler[type] = action;
        }
        
        public void UnBindUIEvent(GameObject go, Action<PointerEventData> action, EUIEvent type = EUIEvent.PointerClick)
        {
            UIEventHandler evt = UIUtility.GetOrAddComponent<UIEventHandler>(go);

            if (evt.EventHandler.TryGetValue(type, out var existingAction))
            {
                existingAction -= action;

                if (existingAction == null)
                    evt.EventHandler.Remove(type);
                else
                    evt.EventHandler[type] = existingAction;
            }
        }
        
        public void ClearAll() => EventHandler?.Clear();
    }
}   