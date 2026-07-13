using UnityEngine;
using GameplayActions;
using Assets.Common;
using Assets.Scripts.Actions;

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
            EventBrocker.Subscribe<EntityInterractionEvent>(OnActionRegistered);
        }

        public override void OnRemove()
        {
            EventBrocker.Unsubscribe<EntityInterractionEvent>(OnActionRegistered);
            base.OnRemove();
        }

        private void OnActionRegistered(InterractionContext context)
        {
            foreach (var criterion in criteria)
                if (criterion != null && !criterion.Matches(context, Owner)) return;
            _counter++;
            if (_counter >= quantity)
            {
                _counter = 0;
                ExecuteResult(context);
            }
        }

        private void ExecuteResult(InterractionContext context)
        {
            GameObject targetObj = executeOnSource ? context.SourceObject : context.TargetObject;
            if (targetObj == null) return;
            Vector3 targetPosition = targetObj.transform.position;
            foreach (var action in resultActions)
                if (action != null) action.Execute(context, targetPosition);
        }
    }
}