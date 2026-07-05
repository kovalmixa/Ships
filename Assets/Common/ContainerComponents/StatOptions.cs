using Assets.Entity.Controllers;
using Assets.Entity.Modifiers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Common
{
    [System.Serializable]
    public class StatOptions
    {
        [field: SerializeField] public List<StatUnit> Stats { get; private set; }

        public List<ModUnit> mods;
        public List<ItemAbilities> abilities;
    }
}
