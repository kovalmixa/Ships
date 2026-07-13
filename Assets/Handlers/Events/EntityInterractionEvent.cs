using GameplayActions;
using Assets.Common;
using Assets.Handlers.Events;
using Assets.Scripts.Actions;

public struct EntityInterractionEvent : IGameplayEvent
{
    public IInteractive Source { get; }

    public IInteractive Target { get; }

    public IActionStruct ActionStruct { get; }
    public float? FinalValue { get; }
    public InterractionType Type { get; }

    public EntityInterractionEvent(InterractionContext interractionContext, float? finalValue = null)
    {
        Source = interractionContext.SourceInterractive;
        Target = interractionContext.TargetInterractive;
        ActionStruct = interractionContext.ActionStruct;
        FinalValue = finalValue;
        Type = interractionContext.Type;
    }
}