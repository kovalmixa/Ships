using UnityEngine;

namespace Assets.Handlers.SceneHandlers
{
    public static class SceneNodesHandler
    {
        public static GameObject GetNode(string name)
        {
            GameObject node = GameObject.Find(name);
            if (node == null)
            {
                node = GameObject.Find("DontDestroyOnLoad").transform.Find(name).gameObject;
                if (node == null) return null;
            }
            return node;
        }
    }
}
