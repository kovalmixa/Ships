using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Entity.DataContainers
{
    public class Projectile : IObject
    {
        public Graphics Graphics { get; set; }
        public Graphics GetGraphics() => Graphics;
        public int LifeTime { get; set; }
        public int Speed { get; set; }
    }
}
