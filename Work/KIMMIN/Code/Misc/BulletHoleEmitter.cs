using System;
using Chipmunk.GameEvents;
using DewmoLib.Dependencies;
using DewmoLib.ObjectPool.RunTime;
using UnityEngine;
using Work.Code.GameEvents;

namespace Work.Code.Misc
{
    public class BulletHoleEmitter : MonoBehaviour
    {
        [SerializeField] private PoolItemSO bulletHole;
        [Inject] private PoolManagerMono _poolManager;
        
        private void Awake()
        {
            EventBus.Subscribe<BulletHoleEvent>(HandleEmitBulletHole);
        }

        private void HandleEmitBulletHole(BulletHoleEvent evt)
        {
            var hole = _poolManager.Pop<BulletHole>(bulletHole);
            hole.InitHole(evt.Position, evt.Normal);
        }
    }
}