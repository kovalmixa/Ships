using Assets.Entity;
using Assets.Entity.Hull;
using UnityEngine;

namespace Assets.InGameMarkers.Scripts
{
    public abstract class ScriptBase : MonoBehaviour, IScript
    {
    public abstract bool Execute(EntityController entityController);

    public abstract bool IsExecuted(EntityController entityController);

    public abstract bool IsFinished(EntityController entityController);
    }
}