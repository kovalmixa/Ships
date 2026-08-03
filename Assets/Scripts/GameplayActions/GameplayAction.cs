using Assets.Common;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    public abstract class ActionDataSO
    {
        /// <summary>
        /// Returns a dictionary of the object's current stats.
        /// Note: Creates a new Dictionary (use for UI/debugging, but not in a hot loop).
        /// </summary>
        public abstract Dictionary<StatType, float> ToStatTypeDict();

        /// <summary>
        /// Fills the passed dictionary without allocating new memory.
        /// </summary>
        public abstract void PopulateStatDict(Dictionary<StatType, float> targetDict);
    }

    [Serializable]
    public struct ActionConfig
    {
        [Tooltip("What action to perform")]
        public AbilityType ActionType;

        [Tooltip("Settings for this action (DamageDataSO, HealDataSO, etc.)")]
        public ActionDataSO ActionData;
    }

    public abstract class GameplayAction
    {
        public abstract void Execute(InteractionContext context, ActionDataSO data, Vector2 targetPos);
        
        public abstract void Execute(InteractionContext context, ActionDataSO data, IInteractive target);
       
        protected virtual Dictionary<IInteractive, Vector2> GetTargetsToExecuteInRange(Vector2 targetPos, float range, int[] layers)
        {
            var colliders = new List<Collider>();
            foreach (int layer in layers)
                colliders.AddRange(Physics.OverlapSphere(targetPos, range, layer));

            var targetsToExecute = new Dictionary<IInteractive, Vector2>();
            foreach (var collider in colliders)
            {
                var target = collider.GetComponent<IInteractive>();
                if (target == null) continue;

                if (!targetsToExecute.ContainsKey(target))
                    targetsToExecute.Add(target, collider.transform.position);
            }
            return targetsToExecute;
        }
    }

    public abstract class GameplayAction<TData> : GameplayAction where TData : ActionDataSO
    {
        public override void Execute(InteractionContext context, ActionDataSO data, Vector2 targetPos)
        {
            if (data is TData typedData) ExecuteAction(context, typedData, targetPos);
            else Debug.LogError($"[GameplayAction] Expected {typeof(TData).Name}, received {data?.GetType().Name}");
        }

        public override void Execute(InteractionContext context, ActionDataSO data, IInteractive target)
        {
            if (data is TData typedData) ExecuteAction(context, typedData, target);
            else Debug.LogError($"[GameplayAction] Expected {typeof(TData).Name}, received {data?.GetType().Name}");
        }

        protected abstract void ExecuteAction(InteractionContext context, TData data, Vector2 targetPos);
        
        protected abstract void ExecuteAction(InteractionContext context, TData data, IInteractive target);
    }
}
