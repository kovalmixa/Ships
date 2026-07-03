using Assets.Entity;
using Assets.Scripts.Actions;

namespace Assets.Common
{
    public interface IInteractive
    {
        void TakeDamage(EntitySnapshot entitySnapshot, Damage damage);

        void TakeHeal(EntitySnapshot entitySnapshot, Heal heal);
    }
}
