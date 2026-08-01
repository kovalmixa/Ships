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
        public GameObject SourceObject { get; private set; }
        public GameObject TargetObject { get; private set; }

        public IInteractive SourceInteractive { get; private set; }
        public IInteractive TargetInteractive { get; private set; }

        public string AbilityId { get; set; }

        public void SetTarget(GameObject gameObject)
        {
            TargetObject = gameObject;
            TargetInteractive = gameObject?.GetComponent<IInteractive>();
        }

        public void SetSource(GameObject gameObject)
        {
            SourceObject = gameObject;
            SourceInteractive = gameObject?.GetComponent<IInteractive>();
        }
    }

    public class ProjectileInteractionContext : InteractionContext
    {
        public ProjectileDataSO Definition { get; set; }
        public Vector2 StartPosition { get; set; }
        public Vector2? TargetPosition { get; set; }
    }
}
