using Assets.Scripts.Actions;
using Entity.Controllers;
using System;
using System.Collections.Generic;

namespace Assets.Entity.Controllers
{
    public enum TriggerType
    {
        None, OnDamage, OnHeal, OnBuffed, OnBuffRemoved, OnShoot, OnActivate
    }

    public class TriggerController
    {
        private readonly global::Entity.Controllers.EntityController _entityController;
        private Dictionary<TriggerType, List<Action<InteractionContext>>> _triggers = new();
        public TriggerController(global::Entity.Controllers.EntityController entityController) => _entityController = entityController;

        public void OnTrigger(TriggerType type, InteractionContext context)
        {
            if (_triggers.TryGetValue(type, out var actions))
                foreach (var action in actions) action?.Invoke(context);
        } 

        public void AddTrigger() { }

        public void RemoveTrigger() { }
    }
}
