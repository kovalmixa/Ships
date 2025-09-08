using Assets.Entity;
using Assets.Entity.Hull;

namespace Assets.InGameMarkers.Scripts
{
    public interface IScript
    {
        bool IsExecuted(EntityController entityController);
        bool Execute(EntityController entityController);
        bool IsFinished(EntityController entityController);
    }
}