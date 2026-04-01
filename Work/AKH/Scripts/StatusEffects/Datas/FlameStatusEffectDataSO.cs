using Code.StatusEffectSystem;
using Code.StatusEffectSystem.StatusEffects;
using Scripts.Entities;
using Scripts.StatusEffects.Effects;
using UnityEngine;

namespace Scripts.StatusEffects.Datas
{
    [CreateAssetMenu(fileName = "FlameStatusEffectDataSO", menuName = "SO/StatusEffect/FlameStatusEffectData", order = 0)]
    public class FlameStatusEffectDataSO : AbstractStatusEffectDataSO
    {
        public override AbstractStatusEffect CreateStatusEffect(Entity target, StatusEffectInfo info)
        {
            return new FlameStatusEffect(target, info);
        }
    }
}
