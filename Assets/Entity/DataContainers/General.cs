using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class General
    {
        public string Name { get; set; }
        public string SizeType { get; set; }
        public string Type { get; set; }
        public int SlotHeight { get; set; } = 1;
        public int SlotWidth { get; set; } = 1;
    }
}
