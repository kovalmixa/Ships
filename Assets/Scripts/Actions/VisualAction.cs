using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Actions;
using UnityEngine;

namespace Actions
{
    public class VisualAction : TemplateActionBase
    {
        private ObjectPoolHandler _effectPool;
        [SerializeField] private string[] _ids;

        private void Awake()
        {
            _effectPool = SceneNodesHandler.GetPoolHandler("EffectPool");
        }

        public override void Execute(InterractionContext interractionContext, Vector3 targetPos){
            if (_effectPool == null) return;
            _effectPool = _effectPool.gameObject.GetComponent<ObjectPoolHandler>();
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