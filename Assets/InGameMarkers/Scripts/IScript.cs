namespace Assets.InGameMarkers.Scripts
{
    public interface IScript
    {
        bool IsExecuted(Entity.Entity entity);
        bool Execute(Entity.Entity entity);
        bool IsFinished(Entity.Entity entity);
    }
}