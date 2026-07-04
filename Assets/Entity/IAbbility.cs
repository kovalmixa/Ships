using Actions;
using Assets.Entity.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Interfaces
{
    public interface IAbbility
    {
        public IReadOnlyList<ItemAbility> RuntimeAbilities { get; }
        public void AddAbility(ItemAbility ability);
        public bool RemoveAbility(ItemAbility ability);

        public void Activate(Vector3 targetPos, TemplateActionBase[] actions);
    }
}
