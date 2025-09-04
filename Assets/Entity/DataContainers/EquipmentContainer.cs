using Newtonsoft.Json;

namespace Assets.Entity.DataContainers
{
    public class EquipmentContainer : IObject
    {
        public General General;
        public string Id { get; set; }

        public float RotationSpeed;

        public ActivationContainer[] OnActivate;
    }
}
