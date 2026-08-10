using Assets.Handlers.Enums;
using Assets.Scripts.GameplayActions.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    public static class ActionProvider
    {
        #region SingleToneInstances

        public static FireProjectileAction FireProjectile { get; } = new();
        public static ExplosionAction Explosion { get; } = new();
        public static HealAction Heal { get; } = new();
        public static DamageAction Damage { get; } = new();
        public static VfxAction Effect { get; } = new();
        public static AudioAction Audio { get; } = new();

        #endregion

        private static readonly Dictionary<AbilityType, GameplayAction> _actionCache = new()
        {
            { AbilityType.FireWeapon, FireProjectile },
            { AbilityType.DropBomb, Explosion },
            { AbilityType.Heal, Heal },
            { AbilityType.FireLaser, Damage }
        };

        public static GameplayAction GetActionByAbility(AbilityType type)
        {
            if (_actionCache.TryGetValue(type, out var action)) return action;
            Debug.LogWarning($"Action for {type} is not implemented in ActionProvider!");
            return null;
        }
    }
}