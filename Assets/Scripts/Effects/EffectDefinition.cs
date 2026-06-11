using Effects;
using Entity.Controllers.GenericController;
using System;

namespace Assets.Scripts.Effects
{

    public enum EffectTargetType
    {
        Owner, // Affects the owner (like higher probability of rare items)
        Hull, // Affects the hull (HP, hull speed)
        Turret, // Affects the turret itself (rotation speed, rate of fire)
        Projectile // Transferred to projectiles (damage, penetration, and igniting effects on impact)
    }

    public abstract class EffectDefinition
    {
        public float Value { get; set; }
        public float Multiplier { get; set; }

        public EffectTargetType Type { get; set; } = EffectTargetType.Owner;
        public string ExactType { get; set; }

        public float Duration;
        public float TickInterval;

        public bool IsPassive
        {
            get { return Duration == 0; }
            private set {}
        }


        public abstract void Execute(EffectInstance instance = null);
    }
}
