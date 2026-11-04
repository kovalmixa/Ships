using Assets.Entity;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Handlers.FileHandlers
{
    public class GameSessionData
    {
        public EntityData entityDataContainer;
        public Vector2 respawnPosition;
        public string locationName;

        public Dictionary<Vector2, InventoryItemData> inventoryItems;
    }
}
