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
        private Vector2[] _fireSectors;
        public Vector2[] FireSectors {
            get => _fireSectors ??  new [] { RotationSector };
            set
            {
                _fireSectors = new Vector2[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i].x > value[i].y)
                    {
                        Vector2 temp;
                        temp.x = value[i].y;
                        temp.y = value[i].x;
                        if (_fireSectors != null) _fireSectors[i] = temp;
                    }
                    else if (_fireSectors != null) _fireSectors[i] = value[i];
                }
                
            }
        }
    }
}
