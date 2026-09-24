using Assets.Common;
using Assets.Handlers.Events;
using Assets.Scripts.Actions;
using FMODUnity;
using GameplayActions;
using System;
using UnityEngine;

namespace Assets.Entity.BuffStatuses.ActiveOnTick
{
    [Serializable]
    public class CountBuff : BuffStatus
    {
        [Header("Counter")]
        [SerializeField] private int quantity = 5;
        private int _counter = 0;

        [Header("Filters (Must all match)")]
        [SerializeField] private InteractionCriterion[] criteria;

        [Header("Result")]
        [SerializeField] private ActionConfig[] resultActions;
        [SerializeField] private bool executeOnSource = false;

        public override void OnApply(IInteractive owner, InteractionContext context)
        {
            base.OnApply(owner, context);
            _counter = 0;
            EventBrocker.Subscribe<EntityInteractionEvent>(OnActionRegistered);
        }

        public override void OnRemove()
        {
            EventBrocker.Unsubscribe<EntityInteractionEvent>(OnActionRegistered);
            base.OnRemove();
        }

        private void OnActionRegistered(EntityInteractionEvent data)
        {
            foreach (var criterion in criteria)
                if (criterion != null && !criterion.Matches(data, Owner)) return;
            _counter++;
            if (_counter >= quantity)
            {
                _counter = 0;
                ExecuteResult(data.Context);
            }
        }

        private void ExecuteResult(InteractionContext context)
        {
            IInteractive target = executeOnSource ? context.SourceInteractive : context.TargetInteractive;
            if (target == null) return;

            foreach (var config in resultActions)
            {
                var action = ActionProvider.GetActionByAbility(config.ActionType);
                if (action != null && config.ActionData != null) action.Execute(context, config.ActionData, target);
                else Debug.LogWarning($"[CountBuff] No action or data configured for {config.ActionType}");
            }
        }
    }
}