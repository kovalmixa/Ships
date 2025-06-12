using System.Collections.Generic;
using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class HullContainer : IObject
    {
        public General General { get; set; }

        public Graphics Graphics { get; set; }

        public Physics Physics { get; set; }

        public HullWeaponProperties[][] Weapons { get; set; }
        public Graphics GetGraphics() => Graphics;
    }
}