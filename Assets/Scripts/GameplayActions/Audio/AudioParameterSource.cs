using Assets.Entity;
using Assets.Entity.Modifiers;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameplayActions.Audio
{
    public interface IAudioParameterSource
    {
        bool TryGetParameter(string key, out float value);
    }

    public class StatsAudioParameterSource : IAudioParameterSource
    {
        private readonly IStats _stats;
        public StatsAudioParameterSource(IStats stats) => _stats = stats;

        public bool TryGetParameter(string key, out float value)
        {
            if (Enum.TryParse<StatType>(key, out var statType))
            {
                value = _stats.GetLifetimeStat(statType);
                return true;
            }
            value = 0f;
            return false;
        }

        public class DictionaryAudioParameterSource : IAudioParameterSource
        {
            private readonly Dictionary<string, float> _values;
            public DictionaryAudioParameterSource(Dictionary<string, float> values) => _values = values;
            public bool TryGetParameter(string key, out float value) => _values.TryGetValue(key, out value);
        }
    }
}
