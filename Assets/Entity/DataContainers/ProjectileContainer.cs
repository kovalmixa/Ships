namespace Assets.Entity.DataContainers
{
    public class ProjectileContainer : IObject
    {
        public General General { get; set; }

        public Graphics Graphics { get; set; }
        public Graphics GetGraphics() => Graphics;
    }
}
