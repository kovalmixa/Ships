using Entity.Controllers.GenericController;

namespace Assets.Entity.AI.Interfaces
{
    public interface IAiBehavior
    {
        void UpdateBehavior(EntityHullSetup entityBodySetup);
    }
}