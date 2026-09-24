using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using System;
using System.Collections.Generic;
using Assets.Entity.BuffStatuses;
using UnityEngine;

namespace Assets.Common
{
    public enum LayerPositioning { Bottom, Top, Toppest }

    [Serializable]
    public struct AbilityUnit
    {
        public Vector2 abilityPosition;
        public AbilityType type;
        public AbilityActivationMode mode;
        public float delay;
        public float globalUsageDelay;
        public uint charges;
        public bool isPassive;
        public int animationID;
        public LayerPositioning drawLayerPositioning;
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
