using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class EquipmentContainer : IObject
    {
        public General General { get; set; }
        public Graphics Graphics { get; set; }
        public string Id { get; set; }
        public Graphics GetGraphics() => Graphics;
        public Physics Physics{ get; set; }
        public ActivationContainer[] OnActivate;
    }
}
