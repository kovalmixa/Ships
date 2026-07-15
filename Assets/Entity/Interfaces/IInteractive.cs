using Assets.Scripts.Actions;

namespace Assets.Common
{
    public interface IInteractive : IObject
    {
        public void AddBuff(InteractionContext interractionContext);

        public void TakeDamage(InteractionContext interractionContext);

        public void TakeHeal(InteractionContext interractionContext);
    }
}
