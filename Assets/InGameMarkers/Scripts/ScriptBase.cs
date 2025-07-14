using UnityEngine;

namespace Assets.InGameMarkers.Scripts
{
    public abstract class ScriptBase : MonoBehaviour, IScript
    {
    public abstract bool Execute(Entity.EntityBody entityBody);

    public abstract bool IsExecuted(Entity.EntityBody entityBody);

    public abstract bool IsFinished(Entity.EntityBody entityBody);
    }
}