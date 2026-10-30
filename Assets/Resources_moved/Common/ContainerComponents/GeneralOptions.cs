using Assets.Handlers.Enums;
using UnityEngine;

namespace Assets.Common
{
    [System.Serializable]
    public struct GeneralOptions
    {
        public string name;
        public string description;
        public Texture icon;
        [field: SerializeField] public SizeType SizeType { get; private set; }
        public InterractLayerType Layer;
        [field: SerializeField] public int SlotHeight { get; private set; }
        [field: SerializeField] public int SlotWidth { get; private set; }
    }
}
