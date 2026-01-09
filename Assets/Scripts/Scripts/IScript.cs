using Entity.Controllers.GenericController;

namespace Scripts
{
    public interface IScript
    {
        bool IsExecuted(EntityController entityController);
        bool Execute(EntityController entityController);
        bool IsFinished(EntityController entityController);
    }
}