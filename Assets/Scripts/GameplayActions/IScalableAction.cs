using Assets.Common;
using Assets.Scripts.Actions;
using GameplayActions;
using UnityEngine;

public interface IScalableAction<TData>
{
    public abstract void ScaleExecute(InteractionContext context, TData data, Vector2 targetPos, float scale);
    public abstract void ScaleExecute(InteractionContext context, TData data, IInteractive target, float scale);
}