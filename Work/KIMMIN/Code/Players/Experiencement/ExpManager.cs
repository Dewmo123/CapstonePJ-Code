using System;
using System.Collections.Generic;
using Chipmunk.ComponentContainers;
using DewmoLib.Dependencies;
using Scripts.Players;
using UnityEngine;
using Work.Code.Core;

namespace Work.Code.Entities.Experiencement
{
    public class ExpManager : MonoSingleton<ExpManager>
    {
        [SerializeField] private float magneticRange;
        [SerializeField] private float collectDistance;
        [SerializeField] private float expPerOrb;
        [Inject] private Player _player;
        
        private readonly List<ExpOrb> _orbs = new();
        private ExpCompo _expCompo;

        private void Start()
        {
            _expCompo = _player.Get<ExpCompo>();
        }

        public void RegisterOrb(ExpOrb orb)
        {
            orb.SubscribeOnCollect(HandleCollect);
            _orbs.Add(orb);
        }

        private void HandleCollect()
        {
            _expCompo.CurrentValue += expPerOrb;
        }

        private void Update()
        {
            foreach (var orb in _orbs)
            {
                if(!orb.CanCollect) continue;
                
                float distance = (_player.transform.position - orb.transform.position).magnitude;
                
                if (distance < magneticRange)
                {
                    Vector3.Lerp(orb.transform.position, _player.transform.position, Time.deltaTime * 10f);
                }

                if (distance < collectDistance)
                {
                    orb.CollectingOrb();
                }
                
            }
        }
    }
}