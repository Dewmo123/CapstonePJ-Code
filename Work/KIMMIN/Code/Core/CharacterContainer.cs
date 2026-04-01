using System;
using Chipmunk.GameEvents;
using DewmoLib.Dependencies;
using UnityEngine;
using Work.Code.GameEvents;
using Work.Code.Setting;
using Work.Code.SkillTree;

namespace Work.Code.Core
{
    [Provide, DefaultExecutionOrder(-5)]
    public class CharacterContainer : MonoBehaviour, IDependencyProvider
    {
        private static CharacterContainer _instance = null;
        
        [SerializeField] private ChrarcterSO defaultCharacter;
        
        public ChrarcterSO Character { get; private set; }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
            
            Character = defaultCharacter;
            EventBus.Subscribe<SelectCharacterEvent>(HandleSelectCharacter);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<SelectCharacterEvent>(HandleSelectCharacter);
        }

        private void HandleSelectCharacter(SelectCharacterEvent evt)
        {
            Character = evt.Character;
        }
    }
}