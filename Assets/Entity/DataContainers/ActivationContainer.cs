using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.InGameMarkers.Scripts;
using JetBrains.Annotations;

namespace Assets.Entity.DataContainers
{
    public class ActivationContainer : IObject
    {
        public Graphics Graphics { get; set; }
        public Graphics GetGraphics() => Graphics;
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public string Type { get; set; }
        public string[] Sounds { get; set; }
        public string[] Textures { get; set; }
        public string EffectType { get; set; }
        public string[] LightColor { get; set; }
        public string[] Effects { get; set; }
        [CanBeNull] public string Projectile { get; set; }
        public float Delay { get; set; }
    }
}
