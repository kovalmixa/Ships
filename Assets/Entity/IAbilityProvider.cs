using Assets.Scripts.Actions;
using UnityEngine;

namespace Assets.Entity
{
    public interface IAbilityProvider
    {
        string AbilityKey { get; }

        void ExecuteAbility(EntitySnapshot entitySnapshot, Vector3 targetPos);
    }
}
