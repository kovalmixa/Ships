using Assets.Common;
using Assets.Handlers.Events;
using UnityEngine;

public struct TileInteractionEvent : IGameplayEvent
{
    public IInteractive Entity { get; }
    public string TileType { get; }
    public Vector2Int GridPosition { get; } 
    //тайл может сам еще взорваться и это тоже нужно отслеживать

    public TileInteractionEvent(IInteractive entity, string tileType, Vector2Int gridPosition)
    {
        Entity = entity;
        TileType = tileType;
        GridPosition = gridPosition;
    }
}