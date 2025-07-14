using System.Collections.Generic;
using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class HullContainer : IObject
    {
        public General General { get; set; }

        public Graphics Graphics { get; set; }

        public Physics Physics { get; set; }

        public HullEquipmentProperties[][] Equipments { get; set; }
        public string Id { get; set; }
        public Graphics GetGraphics() => Graphics;

        public ActivationContainer[] OnActivate;
    }
}