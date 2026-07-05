using Actions;
using Assets.Entity.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Interfaces
{
    public interface IAbbility
    {
        public IReadOnlyList<ItemAbilities> RuntimeAbilities { get; }
        public void AddAbility(ItemAbilities ability);
        public bool RemoveAbility(ItemAbilities ability);

        public void Activate(Vector3 targetPos, TemplateActionBase[] actions);
    }
}
