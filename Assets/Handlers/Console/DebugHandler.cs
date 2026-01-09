using System.Collections.Generic;
using UnityEngine;

namespace Handlers.Console
{
    public class DebugHandler
    {
        private static DebugHandler _instance;

        public static DebugHandler Instance
        {
            get
            {
                if (_instance == null) _instance = new DebugHandler(); 
                return _instance;
            }
        }
        private readonly Dictionary<string, float> _timers = new();
        private GameObject InGameConsole;

        public void Log(string key, string text, float interval)
        {
            if (!_timers.ContainsKey(key))
                _timers[key] = 0f;

            _timers[key] += Time.deltaTime;

            if (_timers[key] >= interval)
            {
                Debug.Log(text);
                _timers[key] = 0f;
            }
        }
    }
}