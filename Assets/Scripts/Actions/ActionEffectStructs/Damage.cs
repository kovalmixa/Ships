using UnityEngine;
using static Actions.DamageAction;

namespace Assets.Common.ActionEffectStructs
{
    public enum DamageType
    {
        None, Explosive, Corrosion, Energy, Fire, Radiation
    }

    public struct Damage
    {
        [SerializeField] public DamageType Type;

        [SerializeField] public float Radius;

        [SerializeField] public float Value;
    }
}
