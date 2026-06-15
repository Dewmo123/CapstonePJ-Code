using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DewmoLib.Dependencies;
using UnityEngine;

namespace Work.Code.PlayerTasks
{
    public class TaskSequenceRunner : MonoBehaviour
    {
        [Inject] private TaskController taskController;
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private float switchDelay = 0.2f;
        [SerializeField] private float removeDelay = 0.1f;

        private readonly List<PlayerTask> _remainingTasks = new();
        private readonly List<PlayerTask[]> _groupTaskSets = new();
        private TaskGroup[] _taskGroups;
        private int _groupIndex;

        private void Awake()
        {
            _taskGroups = GetComponentsInChildren<TaskGroup>(true);
        }

        private void Start()
        {
            if (playOnStart)
                PlayTaskGroups();
        }

        private void OnDestroy()
        {
            if (taskController == null)
                return;

            taskController.OnTaskCompleted -= HandleCompleteTask;
        }

        public void PlayTaskGroups()
        {
            if (taskController == null)
                return;

            BuildTaskGroups();
            _groupIndex = 0;

            if (_groupTaskSets.Count == 0)
                return;

            taskController.OnTaskCompleted -= HandleCompleteTask;
            taskController.OnTaskCompleted += HandleCompleteTask;
            StartCurrentGroup();
        }

        private void HandleCompleteTask(PlayerTask completedTask)
        {
            if (!_remainingTasks.Remove(completedTask) || _remainingTasks.Count > 0)
                return;

            CompleteCurrentGroup().Forget();
        }

        private void BuildTaskGroups()
        {
            _groupTaskSets.Clear();

            foreach (TaskGroup taskParent in _taskGroups)
            {
                _groupTaskSets.Add(taskParent.Tasks);
            }
        }

        private void StartCurrentGroup()
        {
            PlayerTask[] tasks = _groupTaskSets[_groupIndex];
            _remainingTasks.Clear();

            foreach (PlayerTask task in tasks)
            {
                if (task == null)
                    continue;

                _remainingTasks.Add(task);
                taskController.ShowTask(task);
            }

            if (_remainingTasks.Count == 0)
            {
                CompleteCurrentGroup().Forget();
            }
        }

        private async UniTask CompleteCurrentGroup()
        {
            await ClearCurrentGroupTasks();
            _groupIndex++;

            if (_groupIndex >= _groupTaskSets.Count)
            {
                _remainingTasks.Clear();
                taskController.OnTaskCompleted -= HandleCompleteTask;
                return;
            }

            StartCurrentGroup();
        }

        private async UniTask ClearCurrentGroupTasks()
        {
            PlayerTask[] tasks = _groupTaskSets[_groupIndex];

            foreach (var interaction in _taskGroups[_groupIndex].CompleteInteractions)
            {
                interaction.Interact();
            }

            List<PlayerTask> sortedTasks = new List<PlayerTask>();

            foreach (PlayerTask task in tasks)
            {
                if (task == null)
                    continue;

                sortedTasks.Add(task);
            }

            sortedTasks.Sort((a, b) => b.Priority.CompareTo(a.Priority));

            for (int i = 0; i < sortedTasks.Count; i++)
            {
                PlayerTask task = sortedTasks[i];
                taskController.RemoveTask(task);

                if (i < sortedTasks.Count - 1 && await WaitTaskDelay(removeDelay))
                    return;
            }

            await WaitTaskDelay(switchDelay);
        }

        private async UniTask<bool> WaitTaskDelay(float delay)
        {
            return await UniTask.WaitForSeconds(delay, cancellationToken: this.GetCancellationTokenOnDestroy())
                .SuppressCancellationThrow();
        }
    }
}
