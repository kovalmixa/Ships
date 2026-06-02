using Assets.Common;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Actions
{
    public class EffectAction : ActionBase
    {
        private ObjectPoolHandler _effectPool;
        [SerializeField] private string[] _ids;

        public override void Execute(GameObject source, Vector3 targetPos){
            _effectPool = SceneNodesHandler.GetPoolHandler("EffectPool");
            if (!CanActivate(source, targetPos)) return;
            if (_effectPool == null) return;
            ObjectPoolHandler effectPool = _effectPool.gameObject.GetComponent<ObjectPoolHandler>();
            foreach (var id in _ids) SetupEffect(targetPos, id);
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