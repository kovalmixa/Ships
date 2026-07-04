using Actions;
using Assets.Common;
using Assets.Scripts.Actions;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SetBuffAction : TemplateActionBase, IScalableAction
{
    public BuffStatus[] BuffTemplates;
    public string SourceId { get; set; } = "Ability_XXX";
    [SerializeField] public uint Range;
    [SerializeField] public int[] Layers;
    [SerializeField][CanBeNull] public VisualAction VisualAction;

    public override void Execute(InterractionContext interractionContext, Vector3 targetPos)
    {
        VisualAction?.Execute(interractionContext, targetPos);

        var targets = GetTargetsToExecuteInRange(targetPos, Range, Layers);
        foreach (var target in targets.Values) Execute(interractionContext, target);
    }

    public override void Execute(InterractionContext interractionContext, IInteractive target)
    {
        foreach (var template in BuffTemplates)
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