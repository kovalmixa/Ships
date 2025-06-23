using System.Collections;
using System.Collections.Generic;
using Assets.Entity;
using Assets.Entity.DataContainers;
using Assets.InGameMarkers.Actions;
using UnityEngine;

public class ProjectileAttackAction : IGameAction
{
    public void Execute(ActionContext context)
    {
        if (context.TargetPosition == null || string.IsNullOrEmpty(context.ObjectId))
        {
            Debug.LogWarning("Invalid projectile attack parameters.");
            return;
        }

        Vector3 position = context.Source.transform.position;
        Vector3 direction = (context.TargetPosition.Value - position).normalized;
        Debug.Log(context.ObjectId);
        // «десь ты подставл€ешь свой способ спауна снар€да:
        //ProjectileManager.Instance.SpawnProjectile(
        //    context.ObjectId,
        //    position,
        //    direction,
        //    context.Source
        //);
    }
}
