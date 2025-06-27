using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Entity;
using Assets.Entity.DataContainers;
using Assets.Entity.Projectile;
using Assets.Handlers;
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
        ProjectilePoolHandler projectilePool = FindProjectilePool();
        if (projectilePool == null) return;
        if (!ObjectPoolHandler.Objects.ContainsKey(context.ObjectId)) return;
        ProjectileContainer projectileContainer = ObjectPoolHandler.Objects[context.ObjectId] as ProjectileContainer;
        SetupProjectile(projectilePool, projectileContainer, context);

    }

    protected void SetupProjectile(ProjectilePoolHandler projectilePool, ProjectileContainer projectileContainer, ActionContext context)
    { 
        GameObject projectileObj = projectilePool.Get();
        if (projectileObj == null) return;
        Vector3 position = context.Source.transform.position + (Vector3)context.Position;
        Vector3 direction = (context.TargetPosition.Value - position).normalized;
        Vector3 startPosition = context.Source.transform.position + direction * 0.5f;
        projectileObj.transform.position = startPosition;
        projectileObj.transform.rotation = context.Source.transform.rotation;
        projectileObj.GetComponent<Projectile>().ProjectileContainer = projectileContainer;
        projectileObj.GetComponent<Projectile>().Launch(direction);
    }

    protected ProjectilePoolHandler FindProjectilePool()
    {
        try
        {
            GameObject objectPool = GameObject.Find("ObjectPools");
            if(objectPool == null) throw (new Exception("Object pool not found"));
            GameObject projectileObj = objectPool.transform.Find("ProjectilesPool").gameObject;
            if(projectileObj == null) throw (new Exception("Projectile pool not found"));
            return projectileObj.GetComponent<ProjectilePoolHandler>();
        }
        catch (Exception exception)
        {
            Debug.LogWarning(exception.Message);
        }
        return null;
    }

}
