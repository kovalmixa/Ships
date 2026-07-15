using Assets.Common;
using Assets.Handlers.Events;
using Assets.Scripts.Actions;
using GameplayActions;

public struct EntityInteractionEvent : IGameplayEvent
{
    public InteractionContext Context { get; }

    public IInteractive Source { get; }
    public IInteractive Target { get; }
    public IActionStruct ActionStruct { get; }
    public float? FinalValue { get; }
    public InterractionType Type { get; }

    public EntityInteractionEvent(InteractionContext interractionContext, float? finalValue = null)
    {
        Context = interractionContext;
        Source = interractionContext.SourceInterractive;
        Target = interractionContext.TargetInterractive;
        ActionStruct = interractionContext.ActionStruct;
        FinalValue = finalValue;
        Type = interractionContext.Type;
    }
}