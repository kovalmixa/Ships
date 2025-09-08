using UnityEngine;

namespace Assets.Handlers.SceneHandlers
{
    public class GameObjectHandler : MonoBehaviour
    {
        public static GameObjectHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("GameObjectHandler");
                    _instance = go.AddComponent<GameObjectHandler>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
            private set => _instance = value;
        }
        private static GameObjectHandler _instance;

        public void SetRenderLayerOrder(GameObject parent, int value)
        {
            var renderers = parent.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in renderers)
            {
                spriteRenderer.sortingOrder += value;
            }
        }
    }
}
