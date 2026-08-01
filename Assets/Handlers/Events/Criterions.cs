using Assets.Common;
using Assets.Scripts.Actions;
using GameplayActions;
using UnityEngine;

namespace Assets.Handlers.Events
{
    public abstract class InteractionCriterion : ScriptableObject
    {
        public abstract bool Matches(EntityInteractionEvent evt, IInteractive buffOwner);
    }


    [CreateAssetMenu(menuName = "Buffs/Criteria/Owner Is Source")]
    public class OwnerIsSourceCriterion : InteractionCriterion
    {
        public override bool Matches(EntityInteractionEvent evt, IInteractive buffOwner)
        {
            return evt.Source == buffOwner;
        }
    }

    //[CreateAssetMenu(menuName = "Buffs/Criteria/Specific Damage Type")]
    //public class SpecificDamageCriterion : InteractionCriterion
    //{
    //    [SerializeField] private DamageType _damageType;

    //    public override bool Matches(EntityInteractionEvent evt, IInteractive buffOwner)
    //    {
    //        return evt.ActionData is DamageDataSO damage && damage.type == _damageType;
    //    }
    //}
}
