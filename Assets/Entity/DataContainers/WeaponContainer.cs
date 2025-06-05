using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class WeaponContainer : IObject
    {
        public General General { get; set; }
        public Graphics Graphics { get; set; }
        public Physics Physics{ get; set; }
    }
}
