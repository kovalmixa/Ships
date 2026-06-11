using Entity.Controllers.GenericController;

namespace Assets.Scripts.Effects
{
    public class EffectInstance
    {
        public EffectDefinition Definition;

        public float RemainingTime;

        public float TickTimer;

        public EntityController Source;

        public EntityController Target;

        public bool TimeToExecute(float dt)
        {
            RemainingTime -= dt;
            if (Definition.TickInterval > 0)
            {
                TickTimer -= dt;
                if (TickTimer <= 0)
                {
                    TickTimer += Definition.TickInterval;
                    return true;
                }
            }
            return false;
        }
    }
}
