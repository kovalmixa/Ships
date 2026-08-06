using Assets.Handlers.Enums;
using UnityEngine;

namespace Assets.Common
{
    [System.Serializable]
    public struct GeneralOptions
    {
        [field: SerializeField] public SizeType SizeType { get; private set; }
        public LayerType Layer;
        [field: SerializeField] public int SlotHeight { get; private set; }
        [field: SerializeField] public int SlotWidth { get; private set; }
        public Texture icon;
    }
}
