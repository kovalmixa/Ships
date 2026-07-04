using Assets.Common;
using Assets.Entity.Controllers;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class EquipmentContainer : MonoBehaviour, IObject
    {
        public GeneralOptions general;
        public string Id { get; set; }

        public float rotationSpeed; //replace with stats
        public EquipmentSubType type;
        public Dictionary<(StatType Type, bool IsGlobal), float> stats { get; set; }

        public ItemAbility[] baseAbilities = System.Array.Empty<ItemAbility>();
    }
}
