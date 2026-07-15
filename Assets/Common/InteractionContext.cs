using Assets.Common;
using Assets.Entity;
using GameplayActions;
using UnityEngine;

namespace Assets.Scripts.Actions
{

    public enum InterractionType
    {
        None, Damage, FireProjectile, Heal, SetBuff
    }

    public class InteractionContext
    {
        public EntitySnapshot SourceSnapshot { get; set; }
        public IInteractive SourceInterractive { get; set; }
        public IInteractive TargetInterractive { get; set; }
        public string AbilityId { get; set; }
        public GameObject SourceObject { get; set; }
        public GameObject TargetObject { get; set; }
        public InterractionType Type
        {
            get
            {
                switch (ActionStruct)
                {
                    case Damage : return InterractionType.Damage;
                    case ProjectileDefinition : return InterractionType.FireProjectile;
                    case Heal : return InterractionType.Heal;
                    case BuffDefinition : return InterractionType.SetBuff;
                    default : return InterractionType.None;
                }
            }
        }

        public IActionStruct ActionStruct { get; set; }

        public InteractionContext() { }
        public InteractionContext(InteractionContext interractionContext)
        {
            SourceSnapshot = interractionContext.SourceSnapshot;
            SourceInterractive = interractionContext.SourceInterractive;
            TargetInterractive = interractionContext.TargetInterractive;
            AbilityId = interractionContext.AbilityId;
            SourceObject = interractionContext.SourceObject;
            TargetObject = interractionContext.TargetObject;
            ActionStruct = interractionContext.ActionStruct;
        }
    }
}
