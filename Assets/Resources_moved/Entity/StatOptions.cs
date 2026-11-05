using Assets.Handlers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Entity.BuffStatuses;
using UnityEngine;
using Assets.Entity.StatMods;

namespace Assets.Entity
{
    public enum LayerPositioning { Bottom, Top, Toppest }

    [Serializable]
    public struct AbilityUnit : IEquatable<AbilityUnit>
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

        public bool Equals(AbilityUnit other)
        {
            return abilityPosition.Equals(other.abilityPosition) &&
                   type == other.type &&
                   mode == other.mode &&
                   Mathf.Approximately(delay, other.delay) &&
                   Mathf.Approximately(globalUsageDelay, other.globalUsageDelay) &&
                   charges == other.charges &&
                   isPassive == other.isPassive &&
                   animationID == other.animationID &&
                   drawLayerPositioning == other.drawLayerPositioning;
        }

        public override bool Equals(object obj) => obj is AbilityUnit other && Equals(other);

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(abilityPosition);
            hashCode.Add(type);
            hashCode.Add(mode);
            hashCode.Add(delay);
            hashCode.Add(globalUsageDelay);
            hashCode.Add(charges);
            hashCode.Add(isPassive);
            hashCode.Add(animationID);
            hashCode.Add(drawLayerPositioning);
            return hashCode.ToHashCode();
        }

        public static bool operator ==(AbilityUnit left, AbilityUnit right) => left.Equals(right);
        public static bool operator !=(AbilityUnit left, AbilityUnit right) => !left.Equals(right);
    }

    [System.Serializable]
    public struct StatOptions : IEquatable<StatOptions>
    {
        public List<StatUnit> stats;
        public List<ModUnit> mods;
        public List<BuffStatus> buffs;
        public List<AbilityUnit> abilities;

        public bool Equals(StatOptions other)
        {
            return AreListsEqual(stats, other.stats) &&
                   AreListsEqual(mods, other.mods) &&
                   AreListsEqual(buffs, other.buffs) &&
                   AreListsEqual(abilities, other.abilities);
        }

        public override bool Equals(object obj) => obj is StatOptions other && Equals(other);

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            if (stats != null) foreach (var item in stats) hashCode.Add(item);
            if (mods != null) foreach (var item in mods) hashCode.Add(item);
            if (buffs != null) foreach (var item in buffs) hashCode.Add(item);
            if (abilities != null) foreach (var item in abilities) hashCode.Add(item);
            return hashCode.ToHashCode();
        }

        public static bool operator ==(StatOptions left, StatOptions right) => left.Equals(right);
        public static bool operator !=(StatOptions left, StatOptions right) => !left.Equals(right);

        private static bool AreListsEqual<T>(List<T> list1, List<T> list2)
        {
            if (ReferenceEquals(list1, list2)) return true;
            if (list1 is null || list2 is null) return false;
            return list1.SequenceEqual(list2);
        }
    }
}