using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Dimensions
{
    public int X { get; set; }
    public int Y { get; set; }
    [JsonProperty("w")]
    public int Width { get; set; }
    [JsonProperty("h")]
    public int Height { get; set; }
}