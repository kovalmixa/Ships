using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class WeaponContainer : IObject
    {
        [JsonProperty("general")] public General General { get; set; }
        [JsonProperty("graphics")] public Graphics Graphics { get; set; }
        [JsonProperty("physics")] public Physics Physics{ get; set; }
    }
}
