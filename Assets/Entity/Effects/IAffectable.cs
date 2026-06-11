using Assets.Scripts.Effects;
using System.Collections.Generic;

namespace Assets.Entity.Effects
{
    public interface IAffectable
    {
        public List<EffectComponent> Effects { get; set; }
    }
}
