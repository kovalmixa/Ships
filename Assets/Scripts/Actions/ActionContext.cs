using Assets.Scripts.Effects;
using UnityEngine;

namespace Assets.Common.ActionEffectStructs
{
    public class ActionContext
    {
        public readonly EffectComponent[] Effects;
        public readonly GameObject Source;
        public ActionContext(GameObject source, EffectComponent[] effects)
        {
            effects = effects ?? new EffectComponent[0];
            Source = source;
        }
    }
}