using Assets.Common.ActionEffectStructs;
using Assets.Scripts.Effects;

public class BurnEffectDefinition : EffectDefinition
{
    public float DamagePerTick;
    public float Radius;
    public override void Execute(EffectInstance instance)
    {
        instance.Target.hull.TakeDamage(null,
            new Damage
            {
                Value = DamagePerTick,
                Type = DamageType.Fire,
                Radius = Radius,
            });
    }
}