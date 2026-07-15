using UnityEngine;
using GameplayActions;
using Assets.Common;
using Assets.Scripts.Actions;
using Assets.Handlers.Events;

namespace Assets.Entity.BuffStatuses.ActiveOnTick
{
    public class CountBuff : BuffStatus
    {
        [Header("Счетчик")]
        [SerializeField] private int quantity = 5;
        private int _counter = 0;

        [Header("Фильтры (Должны совпасть ВСЕ)")]
        [SerializeField] private InteractionCriterion[] criteria;

        [Header("Результат")]
        [SerializeField] private GameplayAction[] resultActions;
        [SerializeField] private bool executeOnSource = false;

        public override void OnApply(IInteractive owner)
        {
            base.OnApply(owner);
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
                if (criterion != null && !criterion.Matches(data.Context, Owner)) return;
            _counter++;
            if (_counter >= quantity)
            {
                _counter = 0;
                ExecuteResult(data.Context);
            }
        }

        private void ExecuteResult(InteractionContext context)
        {
            GameObject targetObj = executeOnSource ? context.SourceObject : context.TargetObject;
            if (targetObj == null) return;
            Vector3 targetPosition = targetObj.transform.position;
            foreach (var action in resultActions)
                if (action != null) action.Execute(context, targetPosition);
        }
    }
}