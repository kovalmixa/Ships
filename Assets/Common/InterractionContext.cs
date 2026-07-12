using Assets.Common;
using Assets.Entity;
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

        public InterractionContext() { }
        public InterractionContext(InterractionContext interractionContext)
        {
            this.SourceSnapshot = interractionContext.SourceSnapshot;
            this.SourceInterractive = interractionContext.SourceInterractive;
            this.AbilityId = interractionContext.AbilityId;
            this.SourceObject = interractionContext.SourceObject;
            this.target = interractionContext.target;
        }
    }
}
