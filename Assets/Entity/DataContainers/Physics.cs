using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class Physics
    {
        [JsonProperty("mass")]
        public int Mass { get; set; }
    }
}
