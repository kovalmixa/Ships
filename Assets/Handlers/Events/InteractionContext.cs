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

        private GameObject _sourceObject;
        private GameObject _targetObject;

        public GameObject SourceObject => _sourceObject;
        public GameObject TargetObject => _targetObject;


        private IInteractive _sourceInteractive;
        private IInteractive _targetInteractive;

        public IInteractive SourceInterractive => _sourceInteractive;
        public IInteractive TargetInterractive => TargetInterractive;

        public string AbilityId { get; set; }

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
            SetTarget(interractionContext.TargetObject);
            SetSource(interractionContext.SourceObject);
            AbilityId = interractionContext.AbilityId;
            ActionStruct = interractionContext.ActionStruct;
        }
    
        public void SetTarget(GameObject gameObject)
        {
            _targetObject = gameObject;
            _targetInteractive = gameObject.GetComponent<IInteractive>();
        }

        public void SetSource(GameObject gameObject)
        {
            _sourceObject = gameObject;
            _sourceInteractive = gameObject.GetComponent<IInteractive>();
        }
    }
}
