using UnityEngine;

namespace Assets.Entity.DataContainers
{
    public class HullWeaponProperties
    {
        public string Type { get; set; }
        public int Slot { get; set; }
        public string Size { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string RotationSector { get; set; }
        private string _fireSector;
        public string FireSector {
            get => _fireSector ?? RotationSector;
            set => _fireSector = value;
        }
    }
}
