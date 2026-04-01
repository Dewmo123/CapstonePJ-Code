using Code.StatusEffectSystem;
using Code.StatusEffectSystem.StatusEffects;
using Scripts.Entities;
using UnityEngine;
using Work.Code.StatusEffects.Effects;

namespace Work.Code.StatusEffects.Datas
{
    [CreateAssetMenu(fileName = "PoisonStatusEffectData", menuName = "SO/StatusEffect/PoisonStatusEffectData", order = 0)]
    public class PoisonStatusEffectDataSO : AbstractStatusEffectDataSO
    {
        public override AbstractStatusEffect CreateStatusEffect(Entity target, StatusEffectInfo info)
        {
            return new PoisonStatusEffect(target, info);
        }
    }
}