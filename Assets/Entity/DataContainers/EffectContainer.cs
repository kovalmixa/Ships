namespace Assets.Entity.DataContainers
{
    public class EffectContainer : IObject
    {
        public Graphics Graphics { get; set; }
        public string[] Sounds { get; set; }
        public string Id { get; set; }
        public Graphics GetGraphics() => Graphics;
    }
}
