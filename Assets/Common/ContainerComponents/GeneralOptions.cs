using Assets.Handlers.Enums;
using UnityEngine;

namespace Assets.Common
{
    [System.Serializable]
    public class GeneralOptions
    {
        public string name;
        public SizeType sizeType;
        public LayerType layer;
        public int slotHeight = 1;
        public int slotWidth = 1;
        public Texture icon;
    }
}
