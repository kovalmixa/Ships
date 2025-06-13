using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.InGameMarkers.Scripts;

namespace Assets.Entity.DataContainers
{
    public class EquipmentActivationContainer : IObject
    {
        public Graphics Graphics { get; set; }
        public Graphics GetGraphics() => Graphics;
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public string ActivationType { get; set; }
        public string[] ActivateSounds { get; set; }
        public string[] ActivateEffectTextures { get; set; }
        public string ActivationEffectType { get; set; }
        public string[] LightColor { get; set; }
        public string[] Effects { get; set; }
        public int Projectile { get; set; }
    }
}
