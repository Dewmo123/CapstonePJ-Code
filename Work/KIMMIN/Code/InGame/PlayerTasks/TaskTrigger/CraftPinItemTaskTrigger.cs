using System.Collections.Generic;
using DewmoLib.Dependencies;
using Scripts.Players;
using UnityEngine;
using Work.Code.Craft;
using Work.Code.Craft.Installer;
using Work.Code.GameEvents;

namespace Work.Code.PlayerTasks.TaskTrigger
{
    public class CraftPinItemTaskTrigger : PlayerTaskTrigger
    {
        [SerializeField] private CraftTreeUI craftTreeUI;
        [SerializeField] private CraftingTask craftingTaskPrefab;

        [Inject] private CraftPinItemContainer _pinItemContainer;

        private readonly List<CraftingTask> _craftingTasks = new();
        private readonly List<CraftingTask> _waitingRemoveTasks = new();
        private CraftingTask _taskPrefab;
        private CraftTreeSO _targetTree;

        protected override void OnInitTaskTrigger(Player owner)
        {
            _taskPrefab = craftingTaskPrefab != null ? craftingTaskPrefab : GetComponentInChildren<CraftingTask>(true);
        }

        public override void OnDisposeTrigger()
        {
            ClearCraftingTasks();

            if (_taskRunner != null)
                _taskRunner.OnTaskRemoved -= HandleTaskRemoved;

            _owner.LocalEventBus.Unsubscribe<CompleteCraftingEvent>(HandleCompleteCrafting);
        }

        public override void RaisePlayerTask()
        {
            ClearCraftingTasks();
            if (!_pinItemContainer.TryGetFirstTree(out CraftTreeSO tree))
                return;

            _targetTree = tree;
            CreateCraftingTasks(tree);

            if (_craftingTasks.Count == 0)
                return;

            _owner.LocalEventBus.Unsubscribe<CompleteCraftingEvent>(HandleCompleteCrafting);
            _owner.LocalEventBus.Subscribe<CompleteCraftingEvent>(HandleCompleteCrafting);

            if (_taskRunner != null)
            {
                _taskRunner.OnTaskRemoved -= HandleTaskRemoved;
                _taskRunner.OnTaskRemoved += HandleTaskRemoved;
            }

            RaisePlayerTask(_craftingTasks.ToArray());
        }

        private void CreateCraftingTasks(CraftTreeSO tree)
        {
            HashSet<CraftTreeSO> childTrees = new HashSet<CraftTreeSO>();
            int count = tree.isBinary ? 2 : 3;

            for (int i = 1; i <= count && i < tree.nodeList.Count; i++)
            {
                CraftTreeSO childTree = tree.nodeList[i].Tree;
                if (childTree == null || childTree.Item == null || !childTrees.Add(childTree))
                    continue;

                
                CreateCraftingTask(childTree);
            }
            
            CreateCraftingTask(tree);
        }

        private void HandleCompleteCrafting(CompleteCraftingEvent evt)
        {
            if (_targetTree == null || evt.CraftedItem != _targetTree.Item)
                return;

            _owner.LocalEventBus.Unsubscribe<CompleteCraftingEvent>(HandleCompleteCrafting);

            foreach (CraftingTask task in _craftingTasks)
            {
                if (task != null)
                    task.CompleteCraftingTask();
            }
        }

        private void CreateCraftingTask(CraftTreeSO tree)
        {
            CraftingTask task = Instantiate(_taskPrefab, transform);
            task.gameObject.SetActive(true);
            task.name = $"{tree.Item.itemName}_CraftingTask";
            task.InitTask(craftTreeUI, tree.Item, false);
            _craftingTasks.Add(task);
            _waitingRemoveTasks.Add(task);
        }

        private void HandleTaskRemoved(PlayerTask task)
        {
            if (task is not CraftingTask craftingTask || !_waitingRemoveTasks.Remove(craftingTask))
                return;

            if (_waitingRemoveTasks.Count > 0)
                return;

            ClearCraftingTasks();
        }

        private void ClearCraftingTasks()
        {
            if (_owner != null)
                _owner.LocalEventBus.Unsubscribe<CompleteCraftingEvent>(HandleCompleteCrafting);

            if (_taskRunner != null)
                _taskRunner.OnTaskRemoved -= HandleTaskRemoved;

            _targetTree = null;
            _waitingRemoveTasks.Clear();

            foreach (CraftingTask task in _craftingTasks)
            {
                if (task != null)
                    Destroy(task.gameObject);
            }

            _craftingTasks.Clear();
        }
    }
}
