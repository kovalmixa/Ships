using Assets.Entity;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Global.SceneHandlers.GameSession
{
    [Serializable]
    public struct InventorySlot
    {
        public Vector2 position;
        public InventoryItemData itemData;
    }

    public class GameSessionData
    {
        public EntityData entityDataContainer;
        public Vector2 respawnPosition;
        public string locationName;

        public List<InventorySlot> inventoryItems = new();
    }
}
