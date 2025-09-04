using System;

namespace Assets.Entity.DataContainers
{
    [System.Serializable]
    public class EntityContainer
    {
        public HullContainer HullContainer = new();
    }
}
