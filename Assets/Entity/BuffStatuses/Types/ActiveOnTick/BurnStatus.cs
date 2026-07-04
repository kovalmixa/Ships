
using Assets.Scripts.Actions;

namespace Assets.Entity.BuffStatuses.Tick
{
    public class BurnStatus : TickStatus
    {
        private readonly float damage;

        public BurnStatus(float duration,float tickInterval,float damage)
        {
            Duration = duration;
            TickInterval = tickInterval;
            this.damage = damage;
        }

        protected override void OnTick(InterractionContext interractionContext)
        {
            //context.Target.TakeDamage(
            //    interractionContext,
            //    new Damage
            //    {
            //        Value = damage,
            //        Type = DamageType.Fire
            //    });
        }
    }
}
