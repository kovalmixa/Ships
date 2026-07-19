using GameplayActions;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Common
{
    public enum AbilityActivationMode
    {
        Primary,
        Ability,
    }

    [Serializable]
    public struct AbilityUnit
    {
        public AbilityType type;
        public GameplayAction action;
        public AbilityActivationMode mode;
        public float delay;
        public float globalUsageDelay;
        public bool isPassive;
    }

    [System.Serializable]
    public struct StatOptions
    {
        public List<StatUnit> stats;
        public List<ModUnit> mods;
        public List<BuffStatus> buffs;
        public List<AbilityUnit> abilities;
    }
}
