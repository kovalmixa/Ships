using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Graphics
{
    private string texture;

    [JsonProperty("offset_x")] public int OffsetX { get; set; }
    [JsonProperty("offset_y")] public int OffsetY { get; set; }
    [JsonProperty("icon")] public string Icon { get; set; }
    [JsonProperty("texture")] public string Texture
    {
        get => texture ?? Icon;
        set { texture = value; }
    }
    
}
