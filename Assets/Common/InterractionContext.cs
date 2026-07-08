using Assets.Common;
using Assets.Entity;
using Assets.Scripts.Actions.Definitions;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class InterractionContext
    {
        public EntitySnapshot SourceSnapshot { get; set; }
        public IInteractive SourceInterractive { get; set; }
        public string AbilityId { get; set; }
        public GameObject SourceObject { get; set; }
        public GameObject target;

        public IDefinition interractionDefinition;
    }
}
