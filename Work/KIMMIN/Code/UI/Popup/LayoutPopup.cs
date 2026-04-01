namespace Code.UI.Popup
{
    public abstract class LayoutPopup<TData, TCallback> : BasePopup<TData, TCallback> where TCallback : ICallbackData
    {
        public virtual int SortOrder => 0;

        public override void EnableUI(bool isFade = false)
        {
            base.EnableUI(isFade);
            gameObject.SetActive(true);
        }

        public override void DisableUI(bool isFade = false)
        {
            base.DisableUI(isFade);
            gameObject.SetActive(false);
        }
    }
}