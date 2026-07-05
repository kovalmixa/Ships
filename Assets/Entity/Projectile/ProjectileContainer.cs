using Assets.Common;
using Assets.Common.Interfaces;
using Assets.Entity.Controllers;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Projectile
{
    public class ProjectileContainer : MonoBehaviour
    {
        public string Id { get; set; }
        [field: SerializeField] public ProjectileType Type { get; set; }

        public StatOptions statOptions;
    }
}
