using Chipmunk.ComponentContainers;
using Code.SHS.Animations;

namespace Code.SHS.Entities.Enemies
{
    public class Boss : Enemy
    {
        public ParameterAnimator ParamAnimator { get; private set; }

        public override void OnInitialize(ComponentContainer componentContainer)
        {
            base.OnInitialize(componentContainer);
            ParamAnimator = ComponentContainer.GetComponent<ParameterAnimator>();
        }
    }
}
