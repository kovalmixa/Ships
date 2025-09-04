namespace Assets.Entity.DataContainers
{
    public class EffectContainer : IObject
    {
        public string[] Sounds;
        public string Id { get; set; }
        public ActivationContainer[] ActivationContainers;
    }
}
