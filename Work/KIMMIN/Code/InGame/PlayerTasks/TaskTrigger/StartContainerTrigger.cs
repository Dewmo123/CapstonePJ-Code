using Scripts.Players;

namespace Work.Code.PlayerTasks.TaskTrigger
{
    public class StartContainerTrigger : PlayerTaskTrigger
    {
        protected override void OnInitTaskTrigger(Player owner)
        {
            RaisePlayerTask();
        }

        public override void OnDisposeTrigger() { }
    }
}
