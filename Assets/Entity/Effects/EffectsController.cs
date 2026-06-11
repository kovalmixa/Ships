using Assets.Scripts.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Effects
{
    public class EffectsController
    {
        public List<EffectComponent> Effects { get; set; }
        public bool IsDirty { get; set; }

        public List<EffectComponent> GetBuildEffects(IAffectable affectable)
        {
            if (IsDirty)
            {
                Rebuild(affectable);
                IsDirty = false;
            }
            return Effects;
        }

        private void Rebuild(IAffectable affectable)
        {
            //adding effects to Effects
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            foreach (var effect in Effects) effect?.ExecuteEffects(dt);
        }
    }
}
