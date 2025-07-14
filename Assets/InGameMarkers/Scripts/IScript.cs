namespace Assets.InGameMarkers.Scripts
{
    public interface IScript
    {
        bool IsExecuted(Entity.EntityBody entityBody);
        bool Execute(Entity.EntityBody entityBody);
        bool IsFinished(Entity.EntityBody entityBody);
    }
}