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
    public override void Execute(InterractionContext interractionContext, Vector3 targetPos)
    {
        var buffDef = (BuffDefinition) interractionContext.ActionStruct;
        buffDef.visualAction?.Execute(interractionContext, targetPos);
        var targets = GetTargetsToExecuteInRange(targetPos, buffDef.range, buffDef.layers);
        foreach (var target in targets.Values) Execute(interractionContext, target);
    }

    public override void Execute(InterractionContext interractionContext, IInteractive target)
    {
        var buffDef = interractionContext.ActionStruct;
        foreach (var template in buffDef.buffs)
        {
            var instance = Instantiate(template);

            instance.Initialize(
                buffId: template.name,
                sourceId: interractionContext?.AbilityId ?? SourceId,
                duration: template.IsPermanent ? -1f : template.Duration
            );

            target.AddBuff(interractionContext, instance);
        }
    }

    public void ScaleExecute(InterractionContext interractionContext, Vector3 targetPos, float scale)
    {
        throw new System.NotImplementedException();
    }

    public void ScaleExecute(InterractionContext interractionContext, IInteractive target, float scale)
    {
        throw new System.NotImplementedException();
    }
}