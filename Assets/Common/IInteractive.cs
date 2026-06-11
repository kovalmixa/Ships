using Assets.Common.ActionEffectStructs;
using UnityEngine;

namespace Assets.Common
{
    public interface IInteractive
    {
        void TakeDamage(ActionContext context, Damage damage);

        void TakeHeal(ActionContext context, Heal heal);
    }
}
