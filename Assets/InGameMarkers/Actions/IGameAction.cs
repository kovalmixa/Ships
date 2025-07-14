using Assets.InGameMarkers.Actions;

public interface IGameAction
{
    bool IsPassive { get; set; }
    void Execute(ActionContext actionContext);
}
