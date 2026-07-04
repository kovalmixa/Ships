using Assets.Common;
using Assets.Entity.Controllers;
using Assets.Entity.Modifiers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.DataContainers
{
    public class HullContainer : MonoBehaviour, IObject
    {
        public GeneralOptions general;

        public string Id { get; set; }

        public float maxSpeed;//replace

        public float acceleration;//replace

        public float rotationSpeed; //replace

        public uint maxHealth; //replace

        public uint maxEnergy; //replace
        public Dictionary<(StatType Type, bool IsGlobal), float> stats { get; set; }
        public ItemAbility[] BaseAbilities = System.Array.Empty<ItemAbility>();
    }
}