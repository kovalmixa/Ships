using Assets.Handlers.Console;
using System.Collections.Generic;
using UnityEngine;

namespace Handlers.Console
{
    public class DebugHandler
    {
        private static DebugHandler instance;

        public static DebugHandler Instance
        {
            get
            {
                if (instance == null) instance = new DebugHandler(); 
                return instance;
            }
        }
        private readonly Dictionary<string, float> timers = new();
        private InGameConsole inGameConsole;

        public void Log(string key, string text, float interval)
        {
            if (!timers.ContainsKey(key))
                timers[key] = 0f;

            timers[key] += Time.deltaTime;

            if (timers[key] >= interval)
            {
                Debug.Log(text);
                timers[key] = 0f;
            }
        }
    }
}