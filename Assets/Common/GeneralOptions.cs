using UnityEngine;

namespace Assets.Common
{
    [System.Serializable]
    public class GeneralOptions
    {
        public string Name;
        public int SizeType;
        public int Layer;
        public string Class;
        public int SlotHeight = 1;
        public int SlotWidth = 1;
        public Texture Icon;
    }
}
