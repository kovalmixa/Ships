using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Entity.DataContainers
{
    [System.Serializable]
    public class General
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
