using Assets.Common.ActionEffectStructs;
using Entity.Controllers;

namespace Assets.Entity.BuffStatuses
{
    public class StatusContext
    {
        public EntityController Source;

        public EntityController Target;

        public ActionContext ActionContext;
    }
}
