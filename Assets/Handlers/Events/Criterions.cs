using Assets.Common;
using Assets.Scripts.Actions;
using GameplayActions;
using UnityEngine;

namespace Assets.Handlers.Events
{
    public abstract class InteractionCriterion : ScriptableObject
    {
        public abstract bool Matches(InteractionContext context, IInteractive buffOwner);
    }


    [CreateAssetMenu(menuName = "Buffs/Criteria/Owner Is Source")]
    public class OwnerIsSourceCriterion : InteractionCriterion
    {
        public override bool Matches(InteractionContext context, IInteractive buffOwner)
        {
            return context.SourceInterractive == buffOwner;
        }
    }


    [CreateAssetMenu(menuName = "Buffs/Criteria/Specific Action Type")]
    public class SpecificActionCriterion : InteractionCriterion
    {
        [SerializeField] private InterractionType _type;

        public override bool Matches(InteractionContext context, IInteractive buffOwner) => context.Type == _type;
    }

    [CreateAssetMenu(menuName = "Buffs/Criteria/Specific Damage Type")]
    public class SpecificDamageCriterion : InteractionCriterion
    {
        [SerializeField] private DamageType _damageType;

        public override bool Matches(InteractionContext context, IInteractive buffOwner)
        {
            return context.ActionStruct is Damage damage && damage.type == _damageType;
        }
    }
}
