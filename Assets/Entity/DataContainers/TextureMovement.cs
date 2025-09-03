using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Entity.DataContainers
{
    public class TextureMovement
    {
        public float Time { get; set; }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        public bool IsLooped { get; set; }

    }
}
