using Assets.Common;
using Assets.Entity;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class InterractionContext
    {
        public EntitySnapshot Caster { get; set; }
        public IInteractive CasterInteractive { get; set; }
        public string AbilityId { get; set; }
        public GameObject SourceObject { get; set; }
        public GameObject target;
    }
}
