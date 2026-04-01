using System;
using Scripts.Combat.Datas;
using Scripts.Entities;
using UnityEngine;

namespace Scripts.Combat
{
    public struct DamageContext
    {
        public DamageData DamageData;
        public Vector3 HitPoint;
        public Vector3 HitNormal;

        public GameObject Source;
        public Entity Attacker;
    }
    
    public interface IDamageable
    {
        public event Action<float> OnTakeDamage;

        public void TakeDamage(DamageData damageData);
        
        public void ApplyDamage(DamageContext context);
    }
}