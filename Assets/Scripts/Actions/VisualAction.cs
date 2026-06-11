using Assets.Common;
using Assets.Common.ActionEffectStructs;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Actions
{
    public class VisualAction : ActionBase
    {
        private ObjectPoolHandler effectPool;
        [SerializeField] private string[] ids;

        public override void Execute(ActionContext context, Vector3 targetPos){
            var effectPool = SceneNodesHandler.GetPoolHandler("EffectPool");
            if (!CanActivate(context, targetPos)) return;
            if (effectPool == null) return;
            effectPool = effectPool.gameObject.GetComponent<ObjectPoolHandler>();
            foreach (var id in ids) SetupEffect(targetPos, id);
        }

        protected void SetupEffect(Vector3 targetPos, string id)
        {
            var effectPrefab = PrefabLoader.Instance.GetPrefab(id);
            if (!effectPrefab)
            {
                Debug.LogWarning($"unable to load {id}");
                return;
            }
            var spawnedEffect = Instantiate(effectPrefab, targetPos, Quaternion.identity);
        }
    }
}