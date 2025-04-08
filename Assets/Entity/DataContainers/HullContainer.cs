using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using UnityEngine;

public class HullContainer
{
    [JsonProperty("general")]
    public General General { get; set; }

    [JsonProperty("graphics")]
    public Graphics Graphics { get; set; }

    [JsonProperty("physics")]
    public PhysicsData Physics { get; set; }

    [JsonProperty("weapons")]
    public List<HullWeaponProperties> Weapons { get; set; } = new();
}