using System;
using Chipmunk.ComponentContainers;
using Scripts.FSM;
using UnityEngine;

namespace Code.SHS.Entities.Enemies.FSM
{
    public class EnemyStateMachineBehavior : MonoBehaviour, IContainerComponent
    {
        [SerializeField] private EnemyStateEnum _initialState;
        [SerializeField] private StateDataSO[] _stateDatas;
        [SerializeField] private StateMachine<EnemyStateEnum> _stateMachine;
        public ComponentContainer ComponentContainer { get; set; }
        public StateMachine<EnemyStateEnum> StateMachine => _stateMachine;

        public void OnInitialize(ComponentContainer componentContainer)
        {
            _stateMachine = new StateMachine<EnemyStateEnum>(componentContainer, _stateDatas);
        }

        private void Start()
        {
            _stateMachine.ChangeState(_initialState);
        }

        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }
        private void OnDestroy()
        {
            _stateMachine?.Dispose();
        }

        public void ChangeState(EnemyStateEnum newState, bool forced = false)
        {
            _stateMachine?.ChangeState(newState, forced);
        }
    }
}