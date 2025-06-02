using System.Collections.Generic;
using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class HullContainer : IObject
    {
        [JsonProperty("general")] public General General { get; set; }

        [JsonProperty("graphics")] public Graphics Graphics { get; set; }

        [JsonProperty("physics")] public Physics Physics { get; set; }

        [JsonProperty("weapons")] public List<HullWeaponProperties> Weapons { get; set; } = new();
    }
}