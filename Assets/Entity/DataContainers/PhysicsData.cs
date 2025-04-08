using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PhysicsData
{
    [JsonProperty("mass")]
    public int Mass { get; set; }
}
