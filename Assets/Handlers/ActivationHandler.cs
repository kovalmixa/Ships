using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Entity.DataContainers;
using Assets.Handlers;
using Assets.InGameMarkers.Actions;
using Unity.VisualScripting;
using UnityEngine;

public static class ActivationHandler
{
    private static readonly Dictionary<string, IGameAction> _actions = new()
    {
        ["projectileAttack"] = new FireProjectileAction(),
        ["heal"] = new HealAction(),
        ["explosion"] = new ExplosionAction(),
        ["effect"] = new EffectAction()
    };

    public static void Execute(string actionName, ActionContext context)
    {
        if (_actions.TryGetValue(actionName, out var action))
        {
            action.Execute(context);
        }
        else Debug.LogWarning($"Action '{actionName}' not found.");
    }

    public static bool IsPassive(string type)
    {
        IObject obj;
        IGameAction action;
        if (GameObjectsHandler.Objects.TryGetValue(type, out obj))
        {
            EquipmentContainer equipment = obj as EquipmentContainer;
            foreach (var activation in equipment.OnActivate)
            {
                if (!activation.IsPassive) return false;
            }
            return true;
        }
        if (_actions.TryGetValue(type, out action))
        {
            return action.IsPassive;
        }
        return false;
    }
}
