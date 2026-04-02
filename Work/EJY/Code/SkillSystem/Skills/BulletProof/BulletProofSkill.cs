using Chipmunk.ComponentContainers;
using Code.StatusEffectSystem;
using Code.StatusEffectSystem.StatusEffects;
using Entities;
using Scripts.SkillSystem;
using UnityEngine;
namespace Code.SkillSystem.Skills.BulletProof
{
    public class BulletProofSkill : ActiveSkill
    {
        [SerializeField] private BuffSO shieldBuff;
        [SerializeField] private BuffSO dmgIncreaseByShieldBuff; // temp
        [SerializeField] private StatusEffectCreateData damageMultyIncreaseData;
        [SerializeField] private bool isDmgIncreaseByShield;
        private EntityStatusEffect _entityStatusEffect;
        private VFXComponent _vfxComponent;
        private AbstractStatusEffect _bulletProofShieldEffect;
        private bool _isBulletProofVfxPlaying;
        public override void Init(ComponentContainer container)
        {
            base.Init(container);
            _entityStatusEffect = container.Get<EntityStatusEffect>();
            _vfxComponent = container.Get<VFXComponent>();
            _entityStatusEffect.OnStatusEffectReleased -= HandleStatusEffectReleased;
            _entityStatusEffect.OnStatusEffectReleased += HandleStatusEffectReleased;
        }
        [ContextMenu("Dmg Increase By Shield")]
        public void DmgIncreaseByShield() => isDmgIncreaseByShield = true;
        [ContextMenu("Dmg Multy Increase")]
        private void DamageMultyIncrease()
        {
            shieldBuff.statusEffectCreateData.Add(damageMultyIncreaseData);
        }
        [ContextMenu("Temp Use Skill")]
        public override void StartAndUseSkill()
        {
            if (_isBulletProofVfxPlaying == false)
            {
                _vfxComponent.PlayVFX("BulletProof", transform.position, Quaternion.identity);
                _isBulletProofVfxPlaying = true;
            }
            // temp
            if (isDmgIncreaseByShield)
            {
                foreach (var info in dmgIncreaseByShieldBuff.GetStatusEffectInfo())
                {
                    _entityStatusEffect.AddStatusEffect(info);
                }
            }
            foreach (var info in shieldBuff.GetStatusEffectInfo())
            {
                var appliedEffect = _entityStatusEffect.AddStatusEffect(info);
                if (info.StatusEffect == StatusEffectEnum.SHIELD && appliedEffect != null)
                {
                    _bulletProofShieldEffect = appliedEffect;
                }
            }
        }
        private void HandleStatusEffectReleased(AbstractStatusEffect effect)
        {
            if (effect != _bulletProofShieldEffect)
                return;
            if (_isBulletProofVfxPlaying == false)
                return;
            _vfxComponent.StopVFX("BulletProof");
            _bulletProofShieldEffect = null;
            _isBulletProofVfxPlaying = false;
        }
        private void OnDestroy()
        {
            if (_entityStatusEffect == null)
                return;
            _entityStatusEffect.OnStatusEffectReleased -= HandleStatusEffectReleased;
        }
    }
}