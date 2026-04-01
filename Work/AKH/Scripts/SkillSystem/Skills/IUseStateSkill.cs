using Scripts.FSM;

namespace Assets.Work.AKH.Scripts.SkillSystem.Skills
{
    public interface IUseStateSkill
    {
        StateDataSO TargetState { get; set; }
    }
}

