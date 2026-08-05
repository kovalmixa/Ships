using Assets.Common;
using Assets.Entity;
using Assets.Entity.Controllers;
using Assets.Handlers.Enums;
using Entity.Controllers;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public enum InterractionType
    {
        None, Damage, FireProjectile, Heal, SetBuff
    }

    public class InteractionContext
    {
        public ActionDataController ActionDataController { get; }
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

        public InteractionContext() { }

        public InteractionContext(AbilityType key, EntitySnapshot snapshot, 
            GameObject source, ActionDataController dataController)
        {
            SourceSnapshot = snapshot;
            AbilityId = key.ToString();
            SetSource(source);
            ActionDataController = dataController;
        }
    }
}