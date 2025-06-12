using UnityEngine;

namespace Assets.Entity.DataContainers
{
    public class Graphics
    {
        private string _icon;
        private string[] _textures;
        public int X { get; set; }
        public int Y { get; set; }
        public Vector2 Position => new(X, Y);

        public string Icon
        {
            get
            {
                if (_icon == null)
                    return Textures[0];
                return _icon;
            }
            set => _icon = value;
        }

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
