using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class Graphics
    {
        private string _texture;

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public string Icon { get; set; }
        public string Texture
        {
            get => _texture ?? Icon;
            set => _texture = value;
        }
    
    }
}
