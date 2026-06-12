namespace Assets.Entity.BuffStatuses
{
    public abstract class TickStatus : BuffStatus
    {
        public float TickInterval;
        protected float tickTimer;

        public override void Update(StatusContext context, float dt)
        {
            base.Update(context, dt);
            tickTimer -= dt;
            if (tickTimer <= 0)
            {
                tickTimer += TickInterval;
                OnTick(context);
            }
        }

        protected abstract void OnTick(StatusContext context);
    }
}
