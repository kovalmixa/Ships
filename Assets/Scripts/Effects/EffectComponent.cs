using Effects;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Effects
{
    public class EffectComponent
    {
        public int MaxCapacity { get; set; } = 0;

        public List<EffectInstance> EffectInstances { get; private set; }

        public void AddEffects(List<EffectInstance> effects)
        {
            EffectInstances = EffectInstances.Concat(effects).OrderByDescending(eff => eff.Definition.Value).Take(MaxCapacity).ToList();
        }

        public bool IsOverCapacity() => EffectInstances.Count >= MaxCapacity && MaxCapacity != 0;

        public void ExecuteEffects(float deltaTime)
        {
            foreach (var effInst in EffectInstances) {
                var definition = effInst.Definition;
                if (definition.IsPassive) definition.Execute();
                else if (effInst.TimeToExecute(deltaTime)) definition.Execute(); 
            }
        }
    }
}
