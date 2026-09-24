using Assets.Common;
using Assets.Scripts.Actions;
using System;
using UnityEngine;

namespace Assets.Entity.BuffStatuses
{
    public enum BuffApplicationPolicy
    {
        Stack,            // Несколько одинаковых баффов одновременно
        Replace,          // Заменяем старый на новый
        Refresh,          // Обновляем только длительность текущего
        UniquePerSource   // Только один бафф от конкретного источника (SourceId)
    }

    public enum BuffScope
    {
        Local,  // Действует только на конкретный модуль/оружие
        Global  // Действует на всю сущность (Entity)
    }

    [Serializable]
    public class BuffStatus
    {
        [field: SerializeField] public string Id { get; set; }
        public string SourceId { get; set; }

        [field: SerializeField] public float Duration { get; set; } = -1f;
        public bool IsPermanent => Duration < 0f;

        [field: SerializeField] public BuffScope Scope { get; set; } = BuffScope.Local;
        [field: SerializeField] public BuffApplicationPolicy Policy { get; set; } = BuffApplicationPolicy.Replace;

        public Modifiers.Modifiers modifiers = new();

        private float _lifetime = 0f;
        public IInteractive Owner { get; private set; }
        public InteractionContext InteractionContext { get; private set; }

        public event Action OnRemoved;
        public event Action OnRefreshed;

        public virtual void OnApply(IInteractive owner, InteractionContext context)
        {
            Owner = owner;
            InteractionContext = context;
            _lifetime = 0f;
        }

        public virtual void OnRefresh(float newDuration)
        {
            _lifetime = 0f;
            if (newDuration > Duration) Duration = newDuration;
            OnRefreshed?.Invoke();
        }

        public virtual void OnRemove()
        {
            OnRemoved?.Invoke();
            Owner = null;
        }

        public virtual bool Tick(float deltaTime)
        {
            if (IsPermanent) return false;

            _lifetime += deltaTime;
            return _lifetime >= Duration;
        }
    }
}