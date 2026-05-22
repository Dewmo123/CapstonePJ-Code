using Work.Code.Craft.View;
using Code.Items.ItemInfo;

namespace Work.Code.Craft.Presenter
{
    public class CraftTreePresenter
    {
        private readonly CraftModel _model;
        private readonly CraftTreeView _treeView;
        private CraftTreeSO _currentTree;
        
        public CraftTreePresenter(CraftModel craftModel, CraftTreeView treeView)
        {
            _model = craftModel;
            _treeView = treeView;

            _treeView.RequestItemCount += HandleGetItemCount;
            _treeView.OnNodeSelected += SelectTree;
            _model.Inventory.InventoryChanged += HandleInventoryChanged;
        }

        private int HandleGetItemCount(ItemDataSO item)
        {
            return _model.Inventory.GetItemCount(item);
        }
        
        public void SelectTree(CraftTreeSO tree)
        {
            _currentTree = tree;
            _treeView.RenderTree(tree, hasAnim: true);
        }

        private void HandleInventoryChanged()
        {
            if (_currentTree == null)
                return;

            _treeView.RenderTree(_currentTree, hasAnim: false);
        }

        public void DisposePresenter()
        {
            _treeView.RequestItemCount -= HandleGetItemCount;
            _treeView.OnNodeSelected -= SelectTree;
            _model.Inventory.InventoryChanged -= HandleInventoryChanged;
        }
    }
}
