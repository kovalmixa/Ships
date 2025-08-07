using Assets.Entity;
using Assets.Entity.DataContainers;
using Assets.Handlers;
using UnityEngine;

namespace Assets.InGameMarkers.Actions
{
    public class EffectAction : IGameAction
    {
        public bool IsPassive { get; set; } = true;
        public void Execute(ActionContext actionContext)
        {
            GameObject objectPool = GameObject.Find("ObjectPools");
            Transform node = objectPool.transform.Find("EffectsPool");
            if (node == null)
            {
                Debug.LogWarning("Pool not found");
                return;
            }
            ObjectPoolHandler effectPool = objectPool.gameObject.GetComponent<ObjectPoolHandler>();
            SetupEffect(effectPool, actionContext);
        }

        protected void SetupEffect(ObjectPoolHandler effectPool, ActionContext actionContext)
        {
            EffectContainer effectContainer = GameObjectsHandler.Objects[actionContext.ObjectId] as EffectContainer;
            Debug.Log(effectContainer.Graphics.Animations[0]);
        }
    }
}