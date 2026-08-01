using Assets.Handlers.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    public static class ActionProvider
    {
        private static readonly Dictionary<AbilityType, GameplayAction> _actionCache = new()
        {
            { AbilityType.FireWeapon, new FireProjectileAction() },
            { AbilityType.DropBomb, new ExplosionAction() },
            { AbilityType.Heal, new HealAction() },
            { AbilityType.FireLaser, new DamageAction() }
        };

        //private static readonly Dictionary<AbilityType, GameplayAction> _actionCache = new()
        //{
        //    { AbilityType.FireWeapon, new FireProjectileAction() },
        //    { AbilityType.DropBomb, new ExplosionAction() },
        //    { AbilityType.Heal, new HealAction() },
        //    { AbilityType.FireLaser, new DamageAction() }
        //};

        public static GameplayAction GetAction(AbilityType type)
        {
            if (_actionCache.TryGetValue(type, out var action)) return action;
            Debug.LogWarning($"Action for {type} is not implemented in ActionProvider!");
            return null;
        }
    }
}