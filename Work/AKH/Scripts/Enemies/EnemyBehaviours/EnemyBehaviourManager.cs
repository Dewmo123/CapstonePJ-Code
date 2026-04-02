using Chipmunk.ComponentContainers;
using Chipmunk.Library.Utility.GameEvents.Local;
using Code.SHS.Entities.Enemies;
using Code.SHS.Entities.Enemies.Behaviors;
using Code.SHS.Entities.Enemies.Events.Local;
using Sirenix.Utilities;
using Scripts.SkillSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Enemies.EnemyBehaviours
{
    public class EnemyBehaviourManager : MonoBehaviour, IContainerComponent, ILocalEventSubscriber<EnemySpawnEvent>
    {
        private List<EnemyBehaviour> _behaviours = new(10);

        public ComponentContainer ComponentContainer { get; set; }
        public EnemyBehaviour CurrentBehaviour { get; private set; }

        private Enemy _enemy;

        public void OnInitialize(ComponentContainer componentContainer)
        {
            _enemy = componentContainer.Get<Enemy>(true);
        }

        public void OnLocalEvent(EnemySpawnEvent spawnEvent)
        {
            foreach (var enemyBehaviorPatch in spawnEvent.EnemyData.behaviourPrefabs)
            {
                if (enemyBehaviorPatch == null)
                    continue;
                EnemyBehaviour behaviour = Instantiate(enemyBehaviorPatch.Value, transform);
                enemyBehaviorPatch.ApplySetter(behaviour);
                _behaviours.Add(behaviour);
                behaviour.Init(_enemy);
            }

            RebuildBehaviourCache();
        }


        private void RebuildBehaviourCache()
        {
            _behaviours = _behaviours
                .Where(behaviour => behaviour != null)
                .OrderBy(behaviour => behaviour.Priority).ToList();
        }

        public EnemyBehaviour GetOptimal()
            => _behaviours.FirstOrDefault(behaviour => behaviour != null && behaviour.Condition());

        public void ExecuteOptimal()
        {
            EnemyBehaviour optimalBehaviour = GetOptimal();
            optimalBehaviour?.Execute();
            CurrentBehaviour = optimalBehaviour;
        }
    }
}