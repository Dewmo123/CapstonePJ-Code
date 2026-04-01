using System;
using Scripts.SkillSystem;
using SHS.Scripts.Summon;
using UnityEngine;

namespace SHS.Scripts.Skills
{
    public class SummonSkill : ActiveSkill
    {
        [SerializeField] private GameObject summonPrefab;
        [SerializeField] private Transform summonTransform;

        private void OnValidate()
        {
            Debug.Assert(summonPrefab.GetComponent<ISummonable>() != null,
                "SummonPrefab does not implement ISummonable");
        }

        public override void UseSkill()
        {
            base.UseSkill();
            Summon();
        }

        private GameObject Summon()
        {
            GameObject summon = null;
            if (summonTransform == null)
            {
                summon = Instantiate(summonPrefab, transform.position, transform.rotation);
            }
            else
            {
                summon = Instantiate(summonPrefab, summonTransform.position, summonTransform.rotation);
            }

            return summon;
        }
    }
}
