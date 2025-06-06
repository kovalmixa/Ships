using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class Graphics
    {
        private string[] _textures;

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public string Icon { get; set; }
        public string[] Textures
        {
            get
            {
                if (_textures == null || _textures.Length == 0)
                    return new[] { Icon };
                return _textures;
            }
            set => _textures = value;
        }
    }
}
