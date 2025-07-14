using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.InGameMarkers.Scripts;
using JetBrains.Annotations;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace Assets.Entity.DataContainers
{
    public class ActivationContainer : IObject
    {
        public Graphics Graphics { get; set; }
        public string Id { get; set; }
        public Graphics GetGraphics() => Graphics;
        public Vector2 Position { get; set; }
        public string Type { get; set; }
        public string[] Sounds { get; set; }
        public string[] Textures { get; set; }
        public string EffectType { get; set; }
        public string[] LightColor { get; set; }
        public string[] Effects { get; set; }
        [CanBeNull] public string Projectile { get; set; }
        public float Delay { get; set; }
        public float Range { get; set; }
        public float DeadRange { get; set; }
        public bool IsPassive { get; set; } = false;
        public float Amount { get; set; }
        private Vector2[] _fireSectors;
        public Vector2[] FireSectors
        {
            get => _fireSectors;
            set
            {
                _fireSectors = new Vector2[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    float min = value[i].x;
                    float max = value[i].y;
                    if (min > max)
                    {
                        (min, max) = (max, min);
                    }
                    _fireSectors[i] = new Vector2(min, max);
                }
            }
        }
        public bool CanActivate(float distance = 0)
        {
            if (distance == 0) return true;
            return distance >= DeadRange && distance <= Range;
        } 
    }
}
