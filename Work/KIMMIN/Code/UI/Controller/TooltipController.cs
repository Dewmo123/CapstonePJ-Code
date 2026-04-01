using System;
using System.Collections.Generic;
using Chipmunk.GameEvents;
using Code.UI.Core;
using Code.UI.Tooltip;
using UnityEngine;
using UnityEngine.UI;
using Work.Code.GameEvents;

namespace Code.UI.Controller
{
    class TooltipState
    {
        public Coroutine DelayRoutine;
        public List<BaseTooltip> Tooltips = new();
    }
    
    public class TooltipController : MonoBehaviour
    {
        [SerializeField] private List<BaseTooltip> tooltipTypes; 
        [SerializeField] private TooltipMover tooltipMover;
        [SerializeField] private Transform tooltipRoot;
        
        private Dictionary<Type, BaseTooltip> _tooltipMap = new();
        private Dictionary<Type, Queue<BaseTooltip>> _pool = new();
        private Dictionary<GameObject, TooltipState> _states = new();
        
        private bool _rebuildFlag;
        
        public RectTransform RootRect => tooltipRoot as RectTransform;

        private void Awake()
        {
            MappingTooltip();
            EventBus.Subscribe<BindTooltipEvent>(HandleBindTooltip);
            EventBus.Subscribe<UnBindTooltipEvent>(HandleUnBindTooltip);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<BindTooltipEvent>(HandleBindTooltip);
            EventBus.Unsubscribe<UnBindTooltipEvent>(HandleUnBindTooltip);
        }

        private void LateUpdate()
        {
            if (_rebuildFlag)
            {
                RebuildLayout();
                _rebuildFlag = false;
            }
        }

        private void MappingTooltip()
        {
            foreach (var tooltip in tooltipTypes)
            {
                if(tooltip == null) continue;
                _tooltipMap.TryAdd(tooltip.DataType, tooltip);
            }
        }
        
        private void HandleBindTooltip(BindTooltipEvent evt)
        {
            UIEventHandler handler = UIUtility.GetOrAddComponent<UIEventHandler>(evt.Go);
            BindEnterTooltip(evt.Go, evt.Data, evt.Delay, handler);
            BindExitTooltip(evt.Go, handler);
        }
        
        private void HandleUnBindTooltip(UnBindTooltipEvent evt)
        {
            UIEventHandler handler = UIUtility.GetOrAddComponent<UIEventHandler>(evt.Go);
            handler?.ClearAll();
            
            if (_states.TryGetValue(evt.Go, out var state))
            {
                HideTooltip(evt.Go, state);
            }
        }

        public void BindEnterTooltip<TData>(GameObject go, Func<TData> dataCallback, float delay, UIEventHandler handler)
        {
            handler.BindUIEvent(go, _ => {
                var state = GetState(go);
                StopDelayRoutine(go, state);

                var data = dataCallback.Invoke();
                if (data == null) return;
                ShowTooltip(state, data);
            }, EUIEvent.PointerEnter);
        }

        public void BindExitTooltip(GameObject go, UIEventHandler handler)
        {
            handler.BindUIEvent(go, _ => {
                if (!_states.TryGetValue(go, out var state)) return;
                StopDelayRoutine(go, state);
                HideTooltip(go, state);
            }, EUIEvent.PointerExit);
        }
        
        private void StopDelayRoutine(GameObject go, TooltipState state)
        {
            if (state.DelayRoutine != null)
            {
                StopCoroutine(state.DelayRoutine);
                state.DelayRoutine = null;
            }
        }
        
        private void ShowTooltip(TooltipState state, object data)
        {
            var type = data.GetType();
            if (!_tooltipMap.TryGetValue(type, out var prefab)) return;

            BaseTooltip tooltip;
            if (_pool.TryGetValue(type, out var queue) && queue.Count > 0)
                tooltip = queue.Dequeue();
            else
                tooltip = Instantiate(prefab, tooltipRoot);

            tooltip.ShowTooltip(data);
            state.Tooltips.Add(tooltip);
            SortTooltips(state);
            _rebuildFlag = true;
        }

        private void HideTooltip(GameObject owner, TooltipState state)
        {
            StopDelayRoutine(owner, state);

            foreach (var tooltip in state.Tooltips)
            {
                var type = tooltip.DataType;

                if (!_pool.ContainsKey(type))
                    _pool[type] = new Queue<BaseTooltip>();

                tooltip.HidePopup();
                _pool[type].Enqueue(tooltip);
            }

            state.Tooltips.Clear();
        }
        
        private TooltipState GetState(GameObject go)
        {
            if (!_states.TryGetValue(go, out var state))
            {
                state = new TooltipState();
                _states[go] = state;
            }
            return state;
        }
        
        private void RebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(RootRect);
        }
        
        private void SortTooltips(TooltipState state)
        {
            state.Tooltips.Sort((a, b) => b.SortOrder.CompareTo(a.SortOrder));

            for (int i = 0; i < state.Tooltips.Count; i++)
            {
                state.Tooltips[i].transform.SetSiblingIndex(i);
            }
        }
    }
}