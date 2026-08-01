using Assets.Common;
using Assets.Scripts.Actions;
using GameplayActions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class AbilitiesController
    {
        private bool _isDirty { get; set; } = true;

        public AbilityUnit[] abilities = System.Array.Empty<AbilityUnit>();
        private List<AbilityUnit> _runtimeAbilities = new();
        private List<AbilityUnit> _addedAbilities = new();
        private List<AbilityUnit> _setupAbilities = new();

        private readonly Dictionary<AbilityUnit, float> _abilityCooldowns = new();
        private TotalAbbilitiesController _totalAbbilitiesCtr;
        public AbilitiesController(IEnumerable<AbilityUnit> setupAbilities, TotalAbbilitiesController totalAbbilities)
        {
            _setupAbilities = (List<AbilityUnit>)setupAbilities;
            _totalAbbilitiesCtr = totalAbbilities;
        }

        public void Rebuild()
        {
            _runtimeAbilities.Clear();
            _runtimeAbilities.AddRange(_setupAbilities ?? Enumerable.Empty<AbilityUnit>());
            _runtimeAbilities.AddRange(_addedAbilities);
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
            _totalAbbilitiesCtr.MarkDirty();
            ((List<AbilityUnit>)RuntimeAbilities).Add(ability);
        }

        public virtual bool RemoveAbility(AbilityUnit ability)
        {
            bool removedFromAdded = _addedAbilities.Remove(ability);
            bool removedFromRuntime = ((List<AbilityUnit>)RuntimeAbilities).Remove(ability);

            if (removedFromAdded || removedFromRuntime)
            {
                _isDirty = true;
                _totalAbbilitiesCtr.MarkDirty();
                return true;
            }
            return false;
        }

        public virtual void RemoveAbilities()
        {
            _runtimeAbilities.Clear();
            _isDirty = true;
            _totalAbbilitiesCtr.MarkDirty();
        }

        public virtual bool TryActivate(Vector2 targetPos, AbilityUnit abilityUnit, InteractionContext context)
        {
            var action = ActionProvider.GetActionByAbility(abilityUnit.type);
            if (action == null || !CanActivate(targetPos, abilityUnit)) return false;
            var data = ActionDataFactory.CreateDynamicData(abilityUnit.type, context);
            if (data == null) return false;
            action.Execute(context, data, targetPos);
            return true;
        }

        public virtual bool CanActivate(Vector2 targetPos, AbilityUnit abilityUnit)
        {
            float time = Time.time;
            float delay = abilityUnit.delay;
            if (delay <= 0 || abilityUnit.isPassive) return true;
            _abilityCooldowns.TryGetValue(abilityUnit, out float lastActivationTime);
            if (time - lastActivationTime < delay) return false;
            _abilityCooldowns[abilityUnit] = time;
            return true;
        }
    }
}
