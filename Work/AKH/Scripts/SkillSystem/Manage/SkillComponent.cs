using Chipmunk.ComponentContainers;
using Code.SkillSystem;
using Scripts.Entities;
using Scripts.Players;
using Scripts.SkillSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.SkillSystem.Manage
{
    public abstract class SkillComponent<TSlotType, TSocketType>
        : MonoBehaviour, IContainerComponent, ISkillCompo
        where TSlotType : Enum
        where TSocketType : SkillSocket, new()

    {
        public event Action OnSkillsChanged;
        public ComponentContainer ComponentContainer { get; set; }
        public Dictionary<TSlotType, TSocketType> Sockets { get; private set; } = new();
        public TSocketType CurrentSocket { get; protected set; }
        public Dictionary<SkillDataSO, Skill> Skills => _skills;

        public abstract SkillType SkillType { get; }

        protected readonly Dictionary<SkillDataSO, Skill> _skills = new();
        protected IStateEntity _stateEntity;
        protected Entity _ownerEntity;

        private Dictionary<Skill, SkillSocket> _socketBySkillDic = new();

        public virtual void OnInitialize(ComponentContainer componentContainer)
        {
            foreach (TSlotType slotType in Enum.GetValues(typeof(TSlotType)))
                EnsureSocket(slotType);
            _ownerEntity ??= ComponentContainer.GetSubclassComponent<Entity>();
            _stateEntity = _ownerEntity as IStateEntity;
        }

        public Skill GetSkill(SkillDataSO skillType)
        {
            return _skills.GetValueOrDefault(skillType);
        }

        private void EnsureSocket(TSlotType slot)
        {
            if (!Sockets.ContainsKey(slot))
                Sockets.Add(slot, new TSocketType());
        }

        public virtual void AddSkill(Skill skill)
        {
            SkillDataSO skillType = skill.SkillData;
            if (_skills.ContainsKey(skillType))
                return;
            _skills.Add(skillType, skill);
            OnSkillsChanged?.Invoke();
        }

        public virtual void RemoveSkill(Skill skill)
        {
            SkillDataSO skillType = skill.SkillData;
            _skills.Remove(skillType);
            OnSkillsChanged?.Invoke();
        }

        public void ChangeSkill(SkillDataSO skillData, TSlotType targetSlot)
        {
            if (!Sockets.TryGetValue(targetSlot, out TSocketType socket))
                return;
            if (socket.CurrentSkill != null)
                _socketBySkillDic.Remove(socket.CurrentSkill);
            if (_skills.TryGetValue(skillData, out Skill skill))
            {
                socket.ChangeItem(skill);
                _socketBySkillDic[skill] = socket;
            }
            else
            {
                socket.ChangeItem(null);
            }
        }

        public virtual void ChangeSkill(SkillDataSO skillData, int slotType)
        {
            if (!Enum.IsDefined(typeof(TSlotType), slotType))
                return;
            ChangeSkill(skillData, (TSlotType)Enum.ToObject(typeof(TSlotType), slotType));
        }
    }
}