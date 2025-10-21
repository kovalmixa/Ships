using Assets.Effects;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class EffectAction : ActionBase
    {
        private ObjectPoolHandler _effectPool;
        [SerializeField] private string _id;

        public override void Execute(GameObject source, Vector3 targetPos){
            _effectPool = SceneNodesHandler.GetPoolHandler("EffectPool");
            if (!CanActivate(source, targetPos)) return;
            if (_effectPool == null) return;
            ObjectPoolHandler effectPool = _effectPool.gameObject.GetComponent<ObjectPoolHandler>();
            SetupEffect(targetPos);
        }

        protected void SetupEffect(Vector3 targetPos)
        {
            var effectPrefab = PrefabLoader.Instance.GetPrefab(_id);
            if (!effectPrefab)
            {
                Debug.LogWarning($"unable to load {_id}");
                return;
            }
            var spawnedEffect = Instantiate(effectPrefab, targetPos, Quaternion.identity);
        }
    }
}