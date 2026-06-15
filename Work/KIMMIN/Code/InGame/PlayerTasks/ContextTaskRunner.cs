using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DewmoLib.Dependencies;
using Scripts.Players;
using UnityEngine;
using Work.Code.PlayerTasks.TaskTrigger;

namespace Work.Code.PlayerTasks
{
    [Provide]
    public class ContextTaskRunner : MonoBehaviour, IDependencyProvider
    {
        [Inject] private Player _player;
        [Inject] private TaskController taskController;
        [SerializeField] private float removeDelay = 0.75f;

        private readonly List<PlayerTask> _currentTasks = new();
        private readonly List<PlayerTask> _primaryTasks = new();
        private readonly List<PlayerTask> _remainingTasks = new();
        private readonly List<PlayerTask> _instantRemoveTasks = new();
        private PlayerTaskTrigger[] _taskTriggers;
        private bool _isCompletingTasks;
        private int _taskVersion;

        public event Action OnCurrentTasksRemoved;
        public event Action<PlayerTask> OnTaskRemoved;

        private void Awake()
        {
            _taskTriggers = GetComponentsInChildren<PlayerTaskTrigger>(true);
        }

        private void Start()
        {
            taskController.OnTaskCompleted += HandleTaskCompleted;
            taskController.OnTaskRemoved += HandleTaskRemoved;
            InitializeTriggers();
        }

        private void OnDestroy()
        {
            DisposeTriggers();

            if (taskController == null)
                return;

            taskController.OnTaskCompleted -= HandleTaskCompleted;
            taskController.OnTaskRemoved -= HandleTaskRemoved;
        }

        public bool ShowTasks(PlayerTask[] tasks)
        {
            return ShowTasks(tasks, out _);
        }

        public bool ShowTasks(PlayerTask[] tasks, out bool isInstantRemoveTask)
        {
            if (tasks == null)
            {
                isInstantRemoveTask = false;
                return false;
            }

            isInstantRemoveTask = _primaryTasks.Count > 0 || _isCompletingTasks;

            if (!isInstantRemoveTask)
            {
                _taskVersion++;
                _isCompletingTasks = false;
            }

            bool hasTask = false;

            foreach (PlayerTask task in tasks)
            {
                if (!AddTask(task))
                    continue;

                hasTask = true;

                if (isInstantRemoveTask)
                {
                    if (!task.IsCompleted && !_instantRemoveTasks.Contains(task))
                        _instantRemoveTasks.Add(task);
                }
                else
                {
                    if (!_primaryTasks.Contains(task))
                        _primaryTasks.Add(task);

                    if (!task.IsCompleted && !_remainingTasks.Contains(task))
                        _remainingTasks.Add(task);
                }
            }

            if (!isInstantRemoveTask && _primaryTasks.Count > 0 && _remainingTasks.Count == 0)
                CompleteCurrentTasks(_taskVersion).Forget();

            return hasTask;
        }

        public void ClearActiveTasks()
        {
            _taskVersion++;
            _isCompletingTasks = false;
            _primaryTasks.Clear();
            _remainingTasks.Clear();
            _instantRemoveTasks.Clear();
            ClearCurrentTasks();
        }

        private bool AddTask(PlayerTask task)
        {
            if (task == null)
                return false;

            if (!taskController.HasTask(task) && !taskController.ShowTask(task))
                return false;

            if (!_currentTasks.Contains(task))
                _currentTasks.Add(task);

            return true;
        }

        private void ClearCurrentTasks()
        {
            List<PlayerTask> tasks = new List<PlayerTask>(_currentTasks);

            foreach (PlayerTask task in tasks)
            {
                if (task == null)
                    continue;

                taskController.RemoveTask(task);
            }
        }

        private void RemoveCurrentTask(PlayerTask task)
        {
            _currentTasks.Remove(task);
            _primaryTasks.Remove(task);
            _remainingTasks.Remove(task);
            _instantRemoveTasks.Remove(task);
        }

        private void InitializeTriggers()
        {
            foreach (PlayerTaskTrigger trigger in _taskTriggers)
            {
                if (trigger == null)
                    continue;

                trigger.SetTaskRunner(this);
            }

            foreach (PlayerTaskTrigger trigger in _taskTriggers)
            {
                if (trigger == null)
                    continue;

                trigger.CacheTaskTrigger(_player);
            }

            foreach (PlayerTaskTrigger trigger in _taskTriggers)
            {
                if (trigger == null)
                    continue;

                trigger.InitTaskTrigger();
            }
        }

        private void DisposeTriggers()
        {
            if (_taskTriggers == null)
                return;

            foreach (PlayerTaskTrigger trigger in _taskTriggers)
            {
                if (trigger == null)
                    continue;

                trigger.DisposeTaskTrigger();
                trigger.SetTaskRunner(null);
            }
        }

        private void HandleTaskCompleted(PlayerTask task)
        {
            if (_instantRemoveTasks.Remove(task))
            {
                taskController.RemoveTask(task);
                return;
            }

            if (!_remainingTasks.Remove(task) || _remainingTasks.Count > 0)
                return;

            CompleteCurrentTasks(_taskVersion).Forget();
        }

        private void HandleTaskRemoved(PlayerTask task)
        {
            RemoveCurrentTask(task);
            OnTaskRemoved?.Invoke(task);
        }

        private async UniTask CompleteCurrentTasks(int version)
        {
            if (_isCompletingTasks)
                return;

            _isCompletingTasks = true;
            List<PlayerTask> sortedTasks = new List<PlayerTask>(_primaryTasks);
            sortedTasks.Sort((a, b) => b.Priority.CompareTo(a.Priority));

            for (int i = 0; i < sortedTasks.Count; i++)
            {
                if (version != _taskVersion)
                {
                    _isCompletingTasks = false;
                    return;
                }

                PlayerTask task = sortedTasks[i];
                taskController.RemoveTask(task);

                if (i < sortedTasks.Count - 1 && await WaitTaskDelay(removeDelay))
                    return;
            }

            _primaryTasks.Clear();
            _remainingTasks.Clear();
            _isCompletingTasks = false;
            OnCurrentTasksRemoved?.Invoke();
        }

        private async UniTask<bool> WaitTaskDelay(float delay)
        {
            return await UniTask.WaitForSeconds(delay, cancellationToken: this.GetCancellationTokenOnDestroy())
                .SuppressCancellationThrow();
        }
    }
}
