using JetBrains.Annotations;
using UnityEngine;

namespace Assets.DataContainers
{
    [System.Serializable]
    public class ActivationContainer : MonoBehaviour, IObject
    {
        public string Id { get; set; }
        public Vector2 Position;
        public string Type;
        public string[] Sounds;
        public string[] Textures;
        public string EffectType;
        public string[] LightColor;
        public string[] Effects;
        [CanBeNull] public string ObjectID;
        public float Delay;
        public float Range;
        public float DeadRange;
        public bool IsPassive = false;
        public float Value;
        public Vector2[] FireSectors;
        public bool CanActivate(float distance = 0)
        {
            if (distance <= 0f) return true;
            bool withinRange = Range <= 0f || distance <= Range;
            bool beyondDeadRange = distance >= DeadRange;
            return withinRange && beyondDeadRange;
        } 
    }
}
