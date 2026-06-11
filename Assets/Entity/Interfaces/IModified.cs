using Assets.Scripts.Effects;
using System.Collections.Generic;

public interface IModified
{
    public List<EffectComponent> Effects { get; set; }
    public bool IsDirty { get; set; }

    List<EffectComponent> RebuildEffects();
    public List<EffectComponent> GetBuildEffects();
}