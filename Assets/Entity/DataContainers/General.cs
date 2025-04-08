using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class General
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("sizeType")] public string SizeType { get; set; }
    [JsonProperty("slot_height")] public int SlotHeight { get; set; } = 1;
    [JsonProperty("slot_width")] public int SlotWidth { get; set; } = 1;
}
