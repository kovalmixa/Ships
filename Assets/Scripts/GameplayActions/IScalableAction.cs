using Assets.Common;
using Assets.Scripts.Actions;
using UnityEngine;

public interface IScalableAction
{
    public abstract void ScaleExecute(InterractionContext interractionContext, Vector3 targetPos, float scale);
    public abstract void ScaleExecute(InterractionContext interractionContext, IInteractive target, float scale);
}