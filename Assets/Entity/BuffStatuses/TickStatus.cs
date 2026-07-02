namespace Assets.Entity.BuffStatuses
{
    public abstract class TickStatus : BuffStatus
    {
        public float tickInterval;
        public float tickTimer;
        public abstract void OnTick(StatusContext context);
    }
}
