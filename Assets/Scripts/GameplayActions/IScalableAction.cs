using Assets.Common;
using Assets.Scripts.Actions;
using UnityEngine;

public interface IScalableAction
{
    public abstract void ScaleExecute(InteractionContext interractionContext, Vector3 targetPos, float scale);
    public abstract void ScaleExecute(InteractionContext interractionContext, IInteractive target, float scale);
}