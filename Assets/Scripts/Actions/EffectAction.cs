using Assets.Handlers.SceneHandlers;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Assets.Scripts.Actions
{
    public class EffectAction : MonoBehaviour, IGameAction
    {
        public bool IsPassive { get; set; } = true;
        private Transform _effectPool;
        private void Start()
        {
            GameObject objectPool = SceneNodesHandler.GetNode("ObjectPools");
            _effectPool = objectPool.transform.Find("EffectsPool");
            if (_effectPool == null) Debug.LogWarning("Pool not found");
        }

        public void Execute(GameObject source, Vector3 targetPos){
            if (_effectPool == null) return;
            ObjectPoolHandler effectPool = _effectPool.gameObject.GetComponent<ObjectPoolHandler>();
            //SetupEffect(effectPool);
        }

        protected void SetupEffect(ObjectPoolHandler effectPool, ActionContext actionContext)
        {
            //EffectContainer effectContainer = PrefabLoader.Objects[actionContext.ObjectId] as EffectContainer;
            //Debug.Log(effectContainer.Graphics.Animations[0]);
        }
    }
}