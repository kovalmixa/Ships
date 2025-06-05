using UnityEngine;

namespace Assets.GameObjects.InGameMarkers.Scripts
{
    public abstract class ScriptBase : MonoBehaviour, IScript
    {
    public abstract bool Execute(Entity.Entity entity);

    public abstract bool IsExecuted(Entity.Entity entity);

    public abstract bool IsFinished(Entity.Entity entity);
    }
}