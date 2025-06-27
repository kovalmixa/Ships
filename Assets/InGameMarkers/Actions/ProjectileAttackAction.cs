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

        Transform source = context.Source.transform;
        Vector3 fireOffset = context.Position / 10; // локальное смещение вверх от центра
        Vector3 firePoint = source.TransformPoint(fireOffset); // учитываем поворот и позицию

        Vector3 targetPos = context.TargetPosition ?? source.position;
        Vector3 direction = (targetPos - firePoint).normalized;

        // Устанавливаем позицию снаряда с учётом параллакса
        projectileObj.transform.position = firePoint;

        // Поворот по направлению
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectileObj.transform.rotation = Quaternion.Euler(0, 0, angle + 90f); // +90f — если спрайт вверх

        var projectile = projectileObj.GetComponent<Projectile>();
        projectile.ProjectileContainer = projectileContainer;
        projectile.Launch(direction, null, targetPos, context.Source);
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
