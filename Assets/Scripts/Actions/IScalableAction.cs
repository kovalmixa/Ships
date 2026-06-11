using Assets.Common;
using Assets.Common.ActionEffectStructs;
using UnityEngine;

public interface IScalableAction
{
    public abstract void ScaleExecute(ActionContext context, Vector3 targetPos, float scale);
    public abstract void ScaleExecute(ActionContext context, IInteractive target, float scale);
}