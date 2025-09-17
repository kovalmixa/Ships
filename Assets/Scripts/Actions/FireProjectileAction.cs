using System;
using System.Reflection;
using Assets.Entity.Projectile;
using Assets.Handlers.SceneHandlers;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class FireProjectileAction : ActionBase
    {
        public GameObject ProjectilePrefab;
        public Transform FirePosition;
        private ObjectPoolHandler _poolHandler;

        private void Start()
        {
            IsPassive = false;
            _poolHandler = GetPoolHandler();
        }

        //перенести в класс обработчик
        private static ObjectPoolHandler GetPoolHandler()
        {
            ObjectPoolHandler poolHandler;
            try
            {
                var objectPool = SceneNodesHandler.GetNode("ObjectPools");
                if (objectPool == null) throw new Exception("Master pool node not found");
                var specifiedPool = objectPool.transform.Find("ProjectilesPool").gameObject;
                if (specifiedPool == null) throw new Exception("Projectile pool node not found");
                poolHandler = specifiedPool.GetComponent<ObjectPoolHandler>();
                if (poolHandler == null) throw new Exception("PoolHandler component not found");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return poolHandler;
        }
        //перенести в класс обработчик

        public override void Execute(GameObject source, Vector3 targetPos)
        {
            if (!CanActivate(source, targetPos) || _poolHandler == null) return;
            ObjectPoolHandler projectileObj = _poolHandler.GetComponent<ObjectPoolHandler>();
            Debug.Log("Pew");
            //SetupProjectile(source, targetPos, projectileObj);

        }

        //protected void SetupProjectile(GameObject source, Vector3 targetPos)
        //{
        //    if (ProjectilePrefab == null) return;
        //    Vector3 direction = (targetPos - firePoint).normalized;
        //    projectileObj.transform.position = firePoint;

        //    // Поворот по направлению
        //    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //    projectileObj.transform.rotation = Quaternion.Euler(0, 0, angle + 90f); // +90f — если спрайт вверх

        //    var projectile = projectileObj.GetComponent<Projectile>();
        //    projectile.ProjectileContainer = projectileContainer;
        //    projectile.Launch(direction, null, targetPos, context.Source);
        //}
    }
}
