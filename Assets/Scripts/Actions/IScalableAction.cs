using Assets.Common;
using UnityEngine;

public interface IScalableAction
{
    public abstract void ScaleExecute(GameObject source, Vector3 targetPos, float scale);
    public abstract void ScaleExecute(GameObject source, IInteractive target, float scale);
}