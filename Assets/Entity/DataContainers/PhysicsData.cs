using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class PhysicsData
    {
        [JsonProperty("mass")]
        public int Mass { get; set; }
    }
}
