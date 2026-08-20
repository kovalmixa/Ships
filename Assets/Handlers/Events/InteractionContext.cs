using Assets.Common;
using Assets.Entity;
using Assets.Entity.Controllers;
using Assets.Handlers.Enums;
using Assets.Scripts.GameplayActions.Audio;
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

        public IAudioParameterSource AudioParameterSource { get; private set; }
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

        public Vector2 actionStartPosition { get; }

        public InteractionContext() { }
        
        //For entity controller units
        public InteractionContext(AbilityType key, EntitySnapshot snapshot, 
            GameObject source, ActionDataController dataController, Vector2 startTransform)
        {
            SourceSnapshot = snapshot;
            AbilityId = key.ToString();
            SetSource(source);
            ActionDataController = dataController;
            actionStartPosition = startTransform;
            AudioParameterSource = new StatsAudioParameterSource(SourceInteractive as IStats); 
        }
    }
}