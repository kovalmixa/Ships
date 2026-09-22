using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.GUI.CommandLine
{
    public static class DebugHandler
    {
        private static readonly Dictionary<string, float> _timers = new();
        public static event Action<string> OnLog;
        public static void Log(string key, string text, float interval)
        {
            if (!_timers.ContainsKey(key)) _timers[key] = 0f;
            _timers[key] += Time.deltaTime;
            if (_timers[key] >= interval)
            {
                Debug.Log(text);
                OnLog?.Invoke(text);
                _timers[key] = 0f;
            }
        }
    }
}