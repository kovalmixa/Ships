using Assets.Common;
using Assets.Scripts.Actions;
using UnityEngine;

namespace GameplayActions
{
    public class VfxData : ActionData
    {
        public string[] prefabIds;
    }

    public class VfxAction : GameplayAction<VfxData>
    {
        protected override void ExecuteAction(InteractionContext context, VfxData data, Vector2 targetPos)
        {
            //var effectPool = ObjectPoolHandler.GetInstance(PoolType.Effect);
            //if (effectPool == null) return;
            //effectPool = effectPool.gameObject.GetComponent<ObjectPoolHandler>();
            //foreach (var id in data.prefabIds) SetupEffect(targetPos, id);
        }

        protected override void ExecuteAction(InteractionContext context, VfxData data, IInteractive target)
        {
            if (target is MonoBehaviour monoBehaviour)
            {
                GameObject go = monoBehaviour.gameObject;
                ExecuteAction(context, data, go.transform.position);
            }
        }

        async protected void SetupEffect(Vector3 targetPos, string id)
        {
            //var effectPrefab = await PrefabLoader.Instance.InstantiatePrefabAsync(id);
            //if (!effectPrefab)
            //{
            //    Debug.LogWarning($"unable to load {id}");
            //    return;
            //}
            //var spawnedEffect = GameObject.Instantiate(effectPrefab, targetPos, Quaternion.identity);
        }
    }
}