using Assets.Scripts.Actions;
using Entity.Controllers;
using System;
using System.Collections.Generic;
using TreeEditor;

namespace Assets.Entity.Controllers
{
    public enum TriggerType
    {
        None, OnDamage, OnHeal, OnShoot
    }

    public class TriggerController
    {
        private readonly EntityController _entityController;
        private Dictionary<TriggerType, List<Action<InterractionContext>>> _triggers = new();
        public TriggerController(EntityController entityController) => _entityController = entityController;

        public InterractionContext OnTrigger(TriggerType type, InterractionContext context)
        {
            InterractionContext newContext = new(context);
            if (_triggers.TryGetValue(type, out var actions))
                foreach (var action in actions) action?.Invoke(context);
            return newContext;
        } 

        public void AddTrigger() { }

        public void RemoveTrigger() { }
    }
}
