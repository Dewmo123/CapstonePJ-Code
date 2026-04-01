using Chipmunk.ComponentContainers;
using Chipmunk.Library.Utility.GameEvents.Local;
using Scripts.Combat;
using Scripts.Combat.Datas;
using Scripts.Entities;
using Scripts.FSM;
using Scripts.Players;
using System;
using Chipmunk.GameEvents;
using Code.EnemySpawn;
using Code.ETC;
using Code.GameEvents;
using Code.SHS.Entities.Enemies.Targetings.Events;
using Code.SHS.Entities.Enemies.Combat;
using Code.SHS.Entities.Enemies.Events.Local;
using Code.SHS.Entities.Enemies.FSM;
using Code.SHS.Targetings.Enemies;
using UnityEngine;
using Scripts.Combat.Fovs;
using UnityEngine.Events;
using SHS.Scripts.Combats.Events;

namespace Code.SHS.Entities.Enemies
{
    public class Enemy : Entity, IKnockbackable, IStateEntity, IStunable, IAfterInitialze, IFindable
    {
        [SerializeField] public LayerMask playerLayerMask;
        [SerializeField] private int enemyDropExp = 1;

        public float movingRange = 5;

        public TargetProvider TargetProvider { get; private set; }
        public EnemyStateMachineBehavior StateMachineBehavior { get; private set; }
        public EnemySO EnemyData { get; private set; }
        public NavMovement NavMovement { get; private set; }
        public int SightCount { get; set; }
        [field: SerializeField] public UnityEvent<bool> OnFound { get; private set; }
        [field: SerializeField] public Vector3 SpawnPos { get; private set; }

        private bool _isDead = false;
        private EnemyStunState _stunState;
        private EnemySkillAimState _skillAimState;
        private LocalEventBus _localEventBus;
        private EntityAnimator _entityAnimator;

        public override void OnInitialize(ComponentContainer componentContainer)
        {
            base.OnInitialize(componentContainer);
            TargetProvider = ComponentContainer.GetComponent<TargetProvider>();
            NavMovement = ComponentContainer.GetComponent<NavMovement>(true);
            StateMachineBehavior = ComponentContainer.GetComponent<EnemyStateMachineBehavior>(true);
            _localEventBus = ComponentContainer.GetComponent<LocalEventBus>();
            _entityAnimator = ComponentContainer.GetComponent<EntityAnimator>();
            OnDeadEvent.AddListener(HandleEnemyDead);
        }


        public void AfterInitialize()
        {
            _stunState = StateMachineBehavior.StateMachine.GetState<EnemyStunState>(EnemyStateEnum.Stun);
            _skillAimState = StateMachineBehavior.StateMachine.GetState<EnemySkillAimState>(EnemyStateEnum.AimSkill);
        }

        private void Start()
        {
            OnFound?.Invoke(((IFindable)this).IsFounded);
        }

        private void HandleEnemyDead()
        {
            _isDead = true;
            gameObject.layer = LayerMask.NameToLayer("AvoidEntity");
            ChangeState(EnemyStateEnum.Dead);
            Bus.Raise(new AddPlayerExp(enemyDropExp));
        }

        public void SpawnEnemy(Vector3 position, EnemySO enemyData)
        {
            LocalEventBus localEventBus = ComponentContainer.GetComponent<LocalEventBus>(true);
            localEventBus.Raise(new EnemySpawnEvent(enemyData));
            EnemyData = enemyData;
            SpawnPos = position;
        }

        public void ChangeState(EnemyStateEnum newState, bool forced = false)
            => StateMachineBehavior.ChangeState(newState, forced);

        public void ChangeState(StateDataSO stateData)
        {
            if (Enum.TryParse<EnemyStateEnum>(stateData.enumName, out var newState))
            {
                ChangeState(newState);
            }
        }

        public void Stun(float duration)
        {
            _stunState.SetStunDuration(duration);
            ChangeState(EnemyStateEnum.Stun);
            _localEventBus.Raise(new StunnedEvent(duration));
        }

        public void KnockBack(Vector3 direction, MovementDataSO movementData)
            => NavMovement.KnockBack(direction, movementData);

        public void Founded()
            => OnFound?.Invoke(true);

        public void Escape()
            => OnFound?.Invoke(false);
    }
}