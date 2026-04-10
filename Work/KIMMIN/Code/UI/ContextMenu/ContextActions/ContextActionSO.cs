using UnityEngine;

namespace Work.Code.UI.ContextMenu
{
    [CreateAssetMenu(fileName = "ContextActionSO", menuName = "SO/ContextAction", order = 0)]
    public class ContextActionSO : ScriptableObject
    {
        public Sprite actionIcon;
        public int sortOrder;
        public BaseContextAction contextAction;
    }
}