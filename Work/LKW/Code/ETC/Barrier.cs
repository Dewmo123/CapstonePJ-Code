using System;
using Scripts.Combat;
using Scripts.Combat.Datas;
using Scripts.Entities;
using UnityEngine;

namespace Code.ETC
{
    public class Barrier : MonoBehaviour
    {
        [SerializeField] private LayerMask whatIsBullet;

        [SerializeField] private Animator animator;
        //public event Action<float> OnTakeDamage;

        private bool _isBreak = false;

        private void OnCollisionEnter(Collision collision)
        {
            if (_isBreak == false && ((1 << collision.gameObject.layer) & whatIsBullet) != 0)
            {
                _isBreak = true;
                animator.SetTrigger("BREAK");
            }
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}