using Entity.Controllers;

namespace Scripts
{
    public interface IScript
    {
        bool IsExecuted(Entity.Controllers.EntityController entityController);
        bool Execute(Entity.Controllers.EntityController entityController);
        bool IsFinished(Entity.Controllers.EntityController entityController);
    }
}