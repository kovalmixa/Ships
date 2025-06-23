using System;
using System.Collections;
using System.Collections.Generic;
using Assets.InGameMarkers.Actions;
using UnityEngine;

public static class ActionHandler
{
    private static readonly Dictionary<string, IGameAction> _actions = new()
    {
        ["projectileAttack"] = new ProjectileAttackAction(),
        ["heal"] = new HealAction()
    };

    public static void Execute(string actionName, ActionContext context)
    {
        if (_actions.TryGetValue(actionName, out var action))
        {
            action.Execute(context);
        }
        else Debug.LogWarning($"Action '{actionName}' not found.");
    }
}
