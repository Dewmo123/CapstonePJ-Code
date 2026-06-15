using System.Collections.Generic;
using Scripts.Players;
using UnityEngine;

namespace Work.Code.PlayerTasks.TaskTrigger
{
    public abstract class PlayerTaskTrigger : MonoBehaviour
    {
        [SerializeField] protected PlayerTaskTrigger nextTaskTrigger;
        public PlayerTask[] PlayerTask { get; private set; }

        protected ContextTaskRunner _taskRunner;
        protected Player _owner;

        private readonly List<PlayerTask> _remainingTasks = new();
        private readonly List<PlayerTask> _waitingRemoveTasks = new();
        private bool _waitTaskRemove;

        public void CacheTaskTrigger(Player owner)
        {
            _owner = owner;
            PlayerTask = GetPlayerTasks();
        }

        public void InitTaskTrigger()
        {
            OnInitTaskTrigger(_owner);
        }

        public void DisposeTaskTrigger()
        {
            ClearNextTask();
            OnDisposeTrigger();
        }

        public void SetTaskRunner(ContextTaskRunner taskRunner)
        {
            _taskRunner = taskRunner;
        }

        protected abstract void OnInitTaskTrigger(Player owner);
        public abstract void OnDisposeTrigger();

        public virtual void RaisePlayerTask()
        {
            RaisePlayerTask(PlayerTask);
        }

        protected void RaisePlayerTask(PlayerTask[] playerTasks)
        {
            if (_taskRunner == null)
                return;

            BindNextTask(playerTasks);

            if (!_taskRunner.ShowTasks(playerTasks, out _waitTaskRemove))
                ClearNextTask();
        }

        private void BindNextTask(PlayerTask[] playerTasks)
        {
            ClearNextTask();

            if (nextTaskTrigger == null || playerTasks == null)
                return;

            foreach (PlayerTask task in playerTasks)
            {
                if (task == null)
                    continue;

                _remainingTasks.Add(task);
                _waitingRemoveTasks.Add(task);
                task.OnTaskCompleted += HandleTaskCompleted;
            }
        }

        private void ClearNextTask()
        {
            foreach (PlayerTask task in _remainingTasks)
            {
                if (task == null)
                    continue;

                task.OnTaskCompleted -= HandleTaskCompleted;
            }

            _remainingTasks.Clear();
            _waitingRemoveTasks.Clear();
            _waitTaskRemove = false;

            if (_taskRunner != null)
            {
                _taskRunner.OnCurrentTasksRemoved -= HandleCurrentTasksRemoved;
                _taskRunner.OnTaskRemoved -= HandleTaskRemoved;
            }
        }

        private void HandleTaskCompleted(PlayerTask task)
        {
            if (!_remainingTasks.Remove(task))
                return;

            task.OnTaskCompleted -= HandleTaskCompleted;

            if (_remainingTasks.Count > 0 || _taskRunner == null)
                return;

            if (_waitTaskRemove)
            {
                _taskRunner.OnTaskRemoved -= HandleTaskRemoved;
                _taskRunner.OnTaskRemoved += HandleTaskRemoved;
            }
            else
            {
                _taskRunner.OnCurrentTasksRemoved -= HandleCurrentTasksRemoved;
                _taskRunner.OnCurrentTasksRemoved += HandleCurrentTasksRemoved;
            }
        }

        private void HandleTaskRemoved(PlayerTask task)
        {
            if (!_waitingRemoveTasks.Remove(task))
                return;

            if (_waitingRemoveTasks.Count > 0)
                return;

            if (_taskRunner != null)
                _taskRunner.OnTaskRemoved -= HandleTaskRemoved;

            if (nextTaskTrigger != null)
                nextTaskTrigger.RaisePlayerTask();
        }

        private void HandleCurrentTasksRemoved()
        {
            if (_taskRunner != null)
                _taskRunner.OnCurrentTasksRemoved -= HandleCurrentTasksRemoved;

            if (nextTaskTrigger != null)
                nextTaskTrigger.RaisePlayerTask();
        }

        private PlayerTask[] GetPlayerTasks()
        {
            PlayerTask[] tasks = GetComponentsInChildren<PlayerTask>();
            List<PlayerTask> result = new List<PlayerTask>();

            foreach (PlayerTask task in tasks)
            {
                if (task == null || task.GetComponentInParent<PlayerTaskTrigger>() != this)
                    continue;

                result.Add(task);
            }

            return result.ToArray();
        }
    }
}
