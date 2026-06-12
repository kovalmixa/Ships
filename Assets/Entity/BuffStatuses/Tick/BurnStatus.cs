using Assets.Common.ActionEffectStructs;

namespace Assets.Entity.BuffStatuses.Tick
{
    public class BurnStatus : TickStatus
    {
        private readonly float damage;

        public BurnStatus(float duration,float tickInterval,float damage)
        {
            Duration = duration;
            RemainingTime = duration;
            TickInterval = tickInterval;
            tickTimer = tickInterval;

            this.damage = damage;
        }

        protected override void OnTick(StatusContext context)
        {
            context.Target.TakeDamage(
                context.ActionContext,
                new Damage
                {
                    Value = damage,
                    Type = DamageType.Fire
                });
        }
    }
}
