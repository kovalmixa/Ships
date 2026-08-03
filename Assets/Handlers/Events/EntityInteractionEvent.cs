using Assets.Common;
using Assets.Scripts.Actions;
using GameplayActions;

namespace Assets.Handlers.Events
{
    public struct EntityInteractionEvent : IGameplayEvent
    {
        public InteractionContext Context { get; }
        public ActionData ActionData { get; }

        public IInteractive Source => Context?.SourceInteractive;
        public IInteractive Target => Context?.TargetInteractive;

        public float FinalValue { get; set; }

        public EntityInteractionEvent(InteractionContext context, ActionData actionData, float initialValue = 0f)
        {
            Context = context;
            ActionData = actionData;
            FinalValue = initialValue;
        }
    }
}