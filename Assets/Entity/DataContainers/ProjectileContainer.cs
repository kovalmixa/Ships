namespace Assets.Entity.DataContainers
{
    public class ProjectileContainer : IObject
    {
        public Graphics Graphics { get; set; }
        public Graphics GetGraphics() => Graphics;
        public int LifeTime { get; set; }
        public int Speed { get; set; }
    }
}
