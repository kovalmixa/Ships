using Assets.Common;
using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Actions;
using Assets.Scripts.Markers.Spawner;
using GameplayActions;
using Scripts;
using UnityEngine;

namespace Assets.Scripts.GameplayActions
{
    public enum SpawnPositionMode
    {
        RelativeToSource, // Offset from the source (e.g., a ship's nozzle)
        AtTargetPoint, // At the exact target position (cursor / point)
        FixedWorldPos // Fixed world position
    }

    public interface ISpawnDataProvider
    {
        NpcData NpcData { get; }
        ScriptBase[] Scripts { get; }
        Vector2 Offset { get; }
        SpawnPositionMode PositionMode { get; }
    }

    [System.Serializable]
    public class SpawnData : ActionData
    {
        public NpcData npcData;
        public ScriptBase[] scripts;
        public Vector2 offset;
        public SpawnPositionMode positionMode = SpawnPositionMode.RelativeToSource;
    }

    public class SpawnAction : GameplayAction<SpawnData>
    {
        protected override void ExecuteAction(InteractionContext context, SpawnData data, Vector2 targetPos)
        {
            if (data == null || data.npcData == null) return;

            Vector2 spawnPosition = CalculateSpawnPosition(context, data, targetPos);
            Quaternion spawnRotation = context.SourceObject != null
                ? context.SourceObject.transform.rotation
                : Quaternion.identity;

            var entityController = EntityPoolHandler.Instance.GetEntity();
            if (entityController != null)
            {
                entityController.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
                if (entityController != null && data != null) 
                    entityController.Setup(data.npcData, data.scripts).GetAwaiter().GetResult();
            }
        }

        protected override void ExecuteAction(InteractionContext context, SpawnData data, IInteractive target)
        {
            if (target?.GameObject != null)
                ExecuteAction(context, data, (Vector2)target.GameObject.transform.position);
        }

        private Vector2 CalculateSpawnPosition(InteractionContext context, SpawnData data, Vector2 targetPos)
        {
            switch (data.positionMode)
            {
                case SpawnPositionMode.RelativeToSource:
                    if (context.SourceObject != null)
                        return context.SourceObject.transform.TransformPoint(data.offset + context.actionStartPosition);
                    return targetPos + data.offset;
                case SpawnPositionMode.AtTargetPoint: return targetPos;
                case SpawnPositionMode.FixedWorldPos: return data.offset;
                default: return targetPos;
            }
        }
    }
}