namespace Assets.Entity.DataContainers
{
    public class ProjectileContainer : IObject
    {
        public Graphics Graphics { get; set; }
        public string Id { get; set; }
        public Graphics GetGraphics() => Graphics;
        public int LifeTime { get; set; }
        public int Speed { get; set; }
        public bool IsHoming { get; set; }
        public bool IsBallistic { get; set; }

        public ActivationContainer[] OnActivate;
        public string Explosion { get; set; }
    }
}
