using GameplayActions;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Common
{
    [Serializable]
    public struct AbilityUnit
    {
        public AbilityType type;
        public AbilityActivationMode mode;
        public float delay;
        public float globalUsageDelay;
        public uint charges;
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
