using GameplayActions;
using Assets.Common;
using Assets.Scripts.Actions;
using JetBrains.Annotations;
using UnityEngine;

public struct BuffDefinition : IActionStruct
{
    public BuffStatus[] buffs;
    //public string SourceId { get; set; } = "Ability_XXX";
    [SerializeField] public uint range;
    [SerializeField] public int[] layers;
    [SerializeField][CanBeNull] public VisualAction visualAction;

    public BuffDefinition(BuffStatus[] buffs, uint range, int[] layers, VisualAction visualAction = null){
        this.buffs = buffs;
        this.range = range;
        this.layers = layers;
        this.visualAction = visualAction;
    }

}

public class SetBuffAction : GameplayAction, IScalableAction
{
    public override void Execute(InteractionContext interractionContext, Vector3 targetPos)
    {
        var buffDef = (BuffDefinition) interractionContext.ActionStruct;
        buffDef.visualAction?.Execute(interractionContext, targetPos);
        var targets = GetTargetsToExecuteInRange(targetPos, buffDef.range, buffDef.layers);
        foreach (var target in targets.Values) Execute(interractionContext, target);
    }

    public override void Execute(InteractionContext interractionContext, IInteractive target)
    {
        var buff = interractionContext.ActionStruct as BuffStatus;
        buff.SourceId = interractionContext?.AbilityId ?? interractionContext.SourceSnapshot.Id;
        buff.Duration = buff.IsPermanent ? -1f : buff.Duration;
        target.AddBuffs(interractionContext, buff);
    }

    public void ScaleExecute(InteractionContext interractionContext, Vector3 targetPos, float scale)
    {
        throw new System.NotImplementedException();
    }

    public void ScaleExecute(InteractionContext interractionContext, IInteractive target, float scale)
    {
        throw new System.NotImplementedException();
    }
}