using Assets.Entity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Global.SceneHandlers.GameSession
{
    [Serializable]
    public struct InventorySlotDataSer
    {
        public Vector2 position;
        public string itemId;
        public int quantity;
    }

    public class GameSessionData
    {
        public EntityData entityDataContainer;
        public Vector2 respawnPosition;
        public string locationName;

        public List<InventorySlotDataSer> inventoryItems = new();
    }
}
