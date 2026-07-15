using Assets.Common;
using Assets.Scripts.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class AbilitiesController
    {
        public AbilityUnit[] Abilities = System.Array.Empty<AbilityUnit>();
        private List<AbilityUnit> _runtimeAbilities = new();
        private readonly Dictionary<AbilityUnit, float> _abilityCooldowns = new();
        private TotalAbbilitiesController _totalAbbilitiesCtr;
        public AbilitiesController(TotalAbbilitiesController totalAbbilities) => _totalAbbilitiesCtr = totalAbbilities;

        public void Rebuild(StatOptions statOptions)
        {
            _runtimeAbilities.Clear();
            if (statOptions.abilities != null)
                _runtimeAbilities.AddRange(statOptions.abilities);
        }

        public IReadOnlyList<AbilityUnit> RuntimeAbilities
        {
            get
            {
                _runtimeAbilities ??= new List<AbilityUnit>(Abilities ?? System.Array.Empty<AbilityUnit>());
                return _runtimeAbilities;
            }
        }

        public virtual void AddAbility(AbilityUnit ability)
        {
            if (ability == null) return;
            _runtimeAbilities.Add(ability);
            _totalAbbilitiesCtr.MarkDirty();

            if (ability == null) return;
            ((List<AbilityUnit>)RuntimeAbilities).Add(ability);
        }

        public virtual bool RemoveAbility(AbilityUnit ability) => ((List<AbilityUnit>)RuntimeAbilities).Remove(ability);

        public virtual bool TryActivate(Vector2 targetPos, AbilityUnit abilityUnit, InteractionContext context)
        {
            throw new System.NotImplementedException();
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
