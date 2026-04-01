using UnityEngine.EventSystems;

namespace Code.UI.Core.Interaction
{
    public delegate void OnClickEvent(IClickable clickable);
    
    public interface IClickable
    {
        void OnClick(PointerEventData eventData) { }
    }
}