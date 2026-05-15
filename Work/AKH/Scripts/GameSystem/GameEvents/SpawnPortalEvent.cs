using Chipmunk.GameEvents;
using Code.EnemySpawn;
using DewmoLib.Dependencies;
using DewmoLib.ObjectPool.RunTime;
using UnityEngine;
using Work.Code.MapEvents;
using Random = UnityEngine.Random;

namespace Scripts.GameSystem.GameEvents
{
    public class SpawnPortalEvent : DropStructureEvent
    {
        [SerializeField] private BossSpawner[] targetSpawners;
        [SerializeField] private PoolItemSO structureItem;

        [Inject] private PoolManagerMono _poolManager;

        protected override void StartDropStructureEvent()
        {
            if (!TryGetRandomAreaPoint(out AreaPoint spawnPoint) || targetSpawners == null || targetSpawners.Length <= 0)
                return;

            BossSpawner targetSpawner = targetSpawners[Random.Range(0, targetSpawners.Length)];
            if (targetSpawner == null)
                return;

            var item = RegisterDropStructure(_poolManager.Pop<PortalStructure>(structureItem));
            item.Init(targetSpawner, spawnPoint.Position);

            EventName = $"{spawnPoint.AreaIndex + 1} 지역 텔레포트 활성화";
            EventBus.Raise(new Work.Code.GameEvents.TeleportToMapEvent(
                spawnPoint.AreaIndex,
                spawnPoint.Position,
                targetSpawner.transform.position));
        }
    }
}
