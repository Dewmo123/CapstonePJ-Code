using System;
using System.Collections.Generic;
using Ami.BroAudio;
using Ami.BroAudio.Runtime;
using DewmoLib.Dependencies;
using Scripts.Players;
using UnityEngine;

namespace Work.Code.PlayerTasks
{
    [Provide]
    public class TaskController : MonoBehaviour, IDependencyProvider
    {
        [SerializeField] private SoundID taskClearSound;
        
        [Inject] private Player _player;
        
        private readonly List<PlayerTask> _activeTasks = new();

        public List<PlayerTask> ActiveTasks => _activeTasks;

        public event Action<PlayerTask> OnTaskAdded;
        public event Action<PlayerTask> OnTaskCompleted;
        public event Action<PlayerTask> OnTaskRemoved;

        public bool ShowTask(PlayerTask task)
        {
            if (_player == null || task == null || _activeTasks.Contains(task))
                return false;

            task.InitializeTask(_player);
            task.OnTaskCompleted += HandleCompleteTask;
            _activeTasks.Add(task);
            OnTaskAdded?.Invoke(task);
            task.BeginTask();
            return true;
        }

        public bool RemoveTask(PlayerTask task)
        {
            if (task == null || !_activeTasks.Remove(task))
                return false;

            task.OnTaskCompleted -= HandleCompleteTask;
            task.CancelTask();
            OnTaskRemoved?.Invoke(task);
            return true;
        }

        public bool HasTask(PlayerTask task)
        {
            return task != null && _activeTasks.Contains(task);
        }

        public bool HasTask<T>() where T : PlayerTask
        {
            foreach (PlayerTask task in _activeTasks)
            {
                if (task is T)
                    return true;
            }

            return false;
        }

        private void HandleCompleteTask(PlayerTask completedTask)
        {
            if (!_activeTasks.Contains(completedTask))
                return;

            OnTaskCompleted?.Invoke(completedTask);
            BroAudio.Play(taskClearSound);
        }
    }
}
