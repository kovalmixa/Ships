using Assets.Common;
using Assets.Entity.Modifiers;
using Assets.Handlers.FileHandlers;
using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    public class EffectData : ActionData
    {
        public string[] prefabIds;

        public override void PopulateStatDict(Dictionary<StatType, float> targetDict)
        {
            throw new System.NotImplementedException();
        }

        public override Dictionary<StatType, float> ToStatTypeDict()
        {
            throw new System.NotImplementedException();
        }
    }

    public class EffectAction : GameplayAction<EffectData>
    {
        protected override void ExecuteAction(InteractionContext context, EffectData data, Vector2 targetPos)
        {
            var effectPool = ObjectPoolHandler.GetInstance(PoolType.Effect);
            if (effectPool == null) return;
            effectPool = effectPool.gameObject.GetComponent<ObjectPoolHandler>();
            foreach (var id in data.prefabIds) SetupEffect(targetPos, id);
        }

        protected override void ExecuteAction(InteractionContext context, EffectData data, IInteractive target)
        {
            if (target is MonoBehaviour monoBehaviour)
            {
                GameObject go = monoBehaviour.gameObject;
                ExecuteAction(context, data, go.transform.position);
            }
        }

        protected void SetupEffect(Vector3 targetPos, string id)
        {
            var effectPrefab = PrefabLoader.Instance.GetPrefab(id);
            if (!effectPrefab)
            {
                Debug.LogWarning($"unable to load {id}");
                return;
            }
            var spawnedEffect = GameObject.Instantiate(effectPrefab, targetPos, Quaternion.identity);
        }
    }
}