using System;
using Chipmunk.GameEvents;
using DewmoLib.Dependencies;
using DewmoLib.ObjectPool.RunTime;
using UnityEngine;
using Work.Code.GameEvents;

namespace Work.Code.UI.Misc
{
    public class DamageTextEmitter : MonoBehaviour
    {
        [SerializeField] private PoolItemSO damageTextPool;

        [Inject] private PoolManagerMono _poolMangaer;

        private void Awake()
        {
            EventBus.Subscribe<DamageTextEvent>(HandleShowDamageText);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<DamageTextEvent>(HandleShowDamageText);
        }

        private void HandleShowDamageText(DamageTextEvent evt)
        {
            var damageText = _poolMangaer.Pop<DamageText>(damageTextPool);
            damageText.InitText(evt.Damage, evt.Position);
        }
    }
}