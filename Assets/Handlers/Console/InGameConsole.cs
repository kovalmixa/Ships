using UnityEngine;

namespace Assets.Handlers.Console
{
    public class InGameConsole : MonoBehaviour
    {
        private static InGameConsole instance;
        public static InGameConsole Instance
        {
            get
            {
                if (instance == null) instance = new InGameConsole();
                return instance;
            }
        }
    }
}
