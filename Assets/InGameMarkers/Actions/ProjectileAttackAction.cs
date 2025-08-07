using System;
using Assets.Entity.DataContainers;
using Assets.Entity.Projectile;
using Assets.Handlers;
using Assets.InGameMarkers.Actions;
using UnityEngine;

public class ProjectileAttackAction : IGameAction
{
    public bool IsPassive { get; set; } = false;
    public void Execute(ActionContext context)
    {
        if (context.TargetPosition == null || string.IsNullOrEmpty(context.ObjectId))
        {
            Debug.LogWarning("Invalid projectile attack parameters.");
            return;
        }
        
        GameObject objectPool = GameObject.Find("ObjectPools");
        ObjectPoolHandler projectileObj = objectPool.transform.Find("ProjectilesPool").gameObject.GetComponent<ObjectPoolHandler>();
        if (projectileObj == null)
        {
            Debug.LogWarning("Pool not found"); 
            return;
        }
        if (!GameObjectsHandler.Objects.ContainsKey(context.ObjectId)) return;
        SetupProjectile(projectileObj, context);

    }

    protected void SetupProjectile(ObjectPoolHandler objectPool, ActionContext context)
    {
        ProjectileContainer projectileContainer = GameObjectsHandler.Objects[context.ObjectId] as ProjectileContainer;
        GameObject projectileObj = objectPool.Get();
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
}
