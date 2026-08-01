
using Assets.Scripts.Actions;

namespace Assets.Entity.BuffStatuses.Tick
{

    public enum BurnType{
        Flame, BlackNapalm
    }

    public class Burn : TickStatus
    {
        public float Damage { get; }

        public Burn(float duration,float tickInterval,float damage)
        {
            Duration = duration;
            TickInterval = tickInterval;
            Damage = damage;
        }

        protected override void OnTick()
        {
            //context.Target.TakeDamage(
            //    context,
            //    new Damage
            //    {
            //        Value = Damage,
            //        Type = DamageType.Fire
            //    });
        }
    }
}
