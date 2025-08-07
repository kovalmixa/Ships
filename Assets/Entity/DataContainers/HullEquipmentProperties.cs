using UnityEngine;

namespace Assets.Entity.DataContainers
{
    public class HullEquipmentProperties
    {
        public string Type { get; set; }
        public int Slot { get; set; }
        public string Size { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        private Vector2 _rotationSector;
        public Vector2 RotationSector { 
            get => _rotationSector;
            set
            {
                if (value.x > value.y)
                {
                    _rotationSector.x = value.y;
                    _rotationSector.y = value.x;
                }
                else _rotationSector = value;
            }
        }
        public Vector2[] FireSectors { get; set; }
    }
}
