using System;
using UnityEngine;

namespace Assets.Resources_moved.Common.ContainerComponents
{
    [Serializable]
    public struct InventoryOptions
    {
        [field: SerializeField] public Vector2 SlotSize { get; private set; }
    }
}
