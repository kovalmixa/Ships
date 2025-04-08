using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

public class HullContainer
{
    [JsonProperty("general")]
    public General General { get; set; }

    [JsonProperty("graphics")]
    public Graphics Graphics { get; set; }

    [JsonProperty("physics")]
    public PhysicsData Physics { get; set; }

    [JsonIgnore]
    public List<HullWeaponProperties> Weapons { get; set; } = new List<HullWeaponProperties>();

    [JsonExtensionData]
    private IDictionary<string, JToken> _extraData;

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        foreach (var kvp in _extraData)
        {
            if (kvp.Key.StartsWith("weapon_"))
            {
                var weapon = kvp.Value.ToObject<HullWeaponProperties>();
                if (weapon != null)
                    Weapons.Add(weapon);
            }
        }
    }
}