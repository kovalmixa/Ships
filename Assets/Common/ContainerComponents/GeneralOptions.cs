using Assets.Handlers.Enums;
using UnityEngine;

namespace Assets.Common
{
    [System.Serializable]
    public class GeneralOptions
    {
        public string name;
        [field: SerializeField] public SizeType SizeType { get; private set; }
        public LayerType Layer;
        [field: SerializeField] public int SlotHeight { get; private set; } = 1;
        [field: SerializeField] public int SlotWidth { get; private set; } = 1;
        public Texture icon;
    }
}
