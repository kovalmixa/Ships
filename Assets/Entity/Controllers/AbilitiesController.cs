using Assets.Common;
using Assets.Common.Interfaces;
using Assets.Entity.Controllers.AI.AITypes;
using Assets.Entity.Interfaces;
using Assets.Scripts.Actions;
using GameplayActions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class AbilitiesController : ICrud, IDirty
    {
        public AbilityUnit[] abilities = Array.Empty<AbilityUnit>();
        private readonly List<AbilityUnit> _runtimeAbilities = new();
        private readonly List<AbilityUnit> _addedAbilities = new();
        private readonly List<AbilityUnit> _setupAbilities = new();

        protected readonly Dictionary<AbilityUnit, float> abilityCooldowns = new();
        protected readonly TotalAbbilitiesController totalAbbilitiesCtr;
        protected readonly ActionDataController actionDataController;
        protected readonly IAbbility source;

        public event Action OnChange;
        public event Action OnDelete;
        public event Action OnInsert;

        public AbilitiesController(IEnumerable<AbilityUnit> setupAbilities, TotalAbbilitiesController totalAbbilities,
            ActionDataController actionDataController, IAbbility source)
        {
            _setupAbilities = (List<AbilityUnit>)setupAbilities;
            totalAbbilitiesCtr = totalAbbilities;
            this.actionDataController = actionDataController;
            this.source = source;
            _isDirty = true;
        }

        public void Rebuild()
        {
            _runtimeAbilities.Clear();
            _runtimeAbilities.AddRange(_setupAbilities ?? Enumerable.Empty<AbilityUnit>());
            _runtimeAbilities.AddRange(_addedAbilities);
            _isDirty = false;
        }

        public IReadOnlyList<AbilityUnit> RuntimeAbilities
        {
            get
            {
                if (_isDirty) Rebuild();
                return _runtimeAbilities;
            }
        }


        public virtual void AddAbility(AbilityUnit ability)
        {
            _addedAbilities.Add(ability);
            _isDirty = true;
            totalAbbilitiesCtr.MarkDirty();
            ((List<AbilityUnit>)RuntimeAbilities).Add(ability);
        }

        public virtual bool RemoveAbility(AbilityUnit ability)
        {
            bool removedFromAdded = _addedAbilities.Remove(ability);
            bool removedFromRuntime = ((List<AbilityUnit>)RuntimeAbilities).Remove(ability);

            if (removedFromAdded || removedFromRuntime)
            {
                _isDirty = true;
                totalAbbilitiesCtr.MarkDirty();
                return true;
            }
            return false;
        }

        public virtual void RemoveAbilities()
        {
            _runtimeAbilities.Clear();
            _isDirty = true;
            totalAbbilitiesCtr.MarkDirty();
        }

        public virtual bool TryActivate(Vector2 targetPos, AbilityUnit abilityUnit)
        {
            var gameobject = (source as IInteractive).GameObject;
            var context = new InteractionContext(abilityUnit.type, source.GetSnapshot(), gameobject, actionDataController);
            var action = ActionProvider.GetActionByAbility(abilityUnit.type);
            if (action == null || !CanActivate(targetPos, abilityUnit)) return false;
            var data = context.ActionDataController.GetActionData(abilityUnit.type, context);
            if (data == null) return false;
            action.Execute(context, data, targetPos);
            //EventBrocker.Raise(new EntityInteractionEvent(context));
            return true;
        }

        public virtual bool CanActivate(Vector2 targetPos, AbilityUnit abilityUnit)
        {
            float time = Time.time;
            float delay = abilityUnit.delay;
            if (delay <= 0 || abilityUnit.isPassive) return true;
            abilityCooldowns.TryGetValue(abilityUnit, out float lastActivationTime);
            if (time - lastActivationTime < delay) return false;
            abilityCooldowns[abilityUnit] = time;
            return true;
        }

        #region IDirty

        private bool _isDirty = false;

        public bool IsDirty => _isDirty;

        public void MarkDirty() => _isDirty = true;

        #endregion
    }
}
