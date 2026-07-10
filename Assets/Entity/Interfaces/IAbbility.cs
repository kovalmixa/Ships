using Assets.Common;
using Assets.Scripts.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Interfaces
{
    public interface IAbbility
    {
        public IReadOnlyList<AbilityUnit> RuntimeAbilities { get; }
        public void AddAbility(AbilityUnit ability);
        public bool RemoveAbility(AbilityUnit ability);
        public void Activate(Vector3 targetPos, AbilityUnit abilityUnit, InterractionContext context);
    }
}
