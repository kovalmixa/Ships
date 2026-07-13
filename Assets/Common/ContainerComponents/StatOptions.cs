using GameplayActions;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Common
{
    public enum AbilityActivationMode
    {
        Primary,
        Ability,
    }

    [Serializable]
    public class AbilityUnit
    {
        public AbilityType type;
        public GameplayAction action;
        public AbilityActivationMode mode;
        public float delay;
        public bool isPassive;
    }

    [System.Serializable]
    public class StatOptions
    {
        [field: SerializeField] public List<StatUnit> Stats { get; private set; }

        public List<ModUnit> mods;
        public List<AbilityUnit> abilities;
    }
}
