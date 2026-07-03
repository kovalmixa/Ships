using Assets.Common;
using Assets.Entity;
using UnityEngine;

public interface IScalableAction
{
    public abstract void ScaleExecute(EntitySnapshot entitySnapshot, Vector3 targetPos, float scale);
    public abstract void ScaleExecute(EntitySnapshot entitySnapshot, IInteractive target, float scale);
}