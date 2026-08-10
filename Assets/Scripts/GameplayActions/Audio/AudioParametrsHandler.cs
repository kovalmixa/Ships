using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameplayActions.Audio
{
    public static class AudioParametrsHandler
    {
        public static void Apply(EventInstance evt, List<AudioParameter> parameters, IAudioParameterSource source)
        {
            if (parameters == null) return;
            foreach (var p in parameters)
            {
                float raw = p.fallbackValue;
                if (source != null && source.TryGetParameter(p.sourceKey, out var v)) raw = v;

                float remapped = Remap(raw, p.remapInMin, p.remapInMax, p.remapOutMin, p.remapOutMax);
                evt.setParameterByName(p.fmodParameterName, remapped);
            }
        }

        private static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
        {
            if (Mathf.Approximately(inMax, inMin)) return outMin;
            float t = Mathf.InverseLerp(inMin, inMax, value);
            return Mathf.Lerp(outMin, outMax, t);
        }
    }
}
