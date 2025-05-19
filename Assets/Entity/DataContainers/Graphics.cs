using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class Graphics
    {
        private string _texture;

        [JsonProperty("offset_x")] public int OffsetX { get; set; }
        [JsonProperty("offset_y")] public int OffsetY { get; set; }
        [JsonProperty("icon")] public string Icon { get; set; }
        [JsonProperty("texture")] public string Texture
        {
            get => _texture ?? Icon;
            set { _texture = value; }
        }
    
    }
}
