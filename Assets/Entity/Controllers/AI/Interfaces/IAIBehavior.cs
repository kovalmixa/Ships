using Entity.Controllers;

namespace Assets.Entity.AI.Interfaces
{
    public interface IAiBehavior
    {
        void UpdateBehavior(EntityHullSetup entityBodySetup);
    }
}