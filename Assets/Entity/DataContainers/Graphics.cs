using UnityEngine;

namespace Assets.Entity.DataContainers
{
    public class Graphics
    {
        private string _icon;
        private string[] _textures;

        public Vector2 Position { get; set; }

        public string Icon
        {
            get
            {
                if (_icon == null && _textures != null)
                    return Textures[0];
                return _icon;
            }
            set => _icon = value;
        }
        
        public string[] Textures
        {
            get
            {
                if ((_textures == null || _textures.Length == 0) && _icon != null)
                    return new[] { Icon };
                return _textures;
            }
            set => _textures = value;
        }

        public string[] Animations { get; set; }

        //private Vector2 size;
        //public Vector2 Size
        //{
        //    get
        //    {
        //        if (size == Vector2.zero) size = Vector2.one;
        //        return size;
        //    }
        //    set
        //    {
        //        if (size == Vector2.zero) return;
        //        size = value;
        //    } 
        //}
    }
}
