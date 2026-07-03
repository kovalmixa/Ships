using Assets.Entity;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Actions
{
    public class VisualAction : TemplateActionBase
    {
        private ObjectPoolHandler effectPool;
        [SerializeField] private string[] ids;

        public override void Execute(EntitySnapshot entitySnapshot, Vector3 targetPos){
            var effectPool = SceneNodesHandler.GetPoolHandler("EffectPool");
            if (!CanActivate(entitySnapshot, targetPos)) return;
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