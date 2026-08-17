using Assets.Entity;
using Assets.Handlers.CommonParents;
using Assets.Scripts.Actions.VFX;
using Entity.Controllers;
using UnityEngine.Pool;

public class EntityHandler : SingletonPoolHandler<EntityHandler, EntityController>
{
    private readonly IObjectPool<VfxInstance> _pool;

    #region Setup

    protected override void ClearOnSceneChange()
    {
        throw new System.NotImplementedException();
    }

    protected override void Awake()
    {
        base.Awake();
        initialCapacity = 20;
        maxPoolSize = 100;
    }

    #endregion

    public void IntantiateEntity(EntityData data)
    {

    }
}