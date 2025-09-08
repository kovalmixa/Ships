using UnityEngine;

namespace Assets.Handlers.SceneHandlers
{
    public class GameObjectHandler : MonoBehaviour
    {
        public static GameObjectHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        public void SetRenderLayerOrder(GameObject parent, int value)
        {
            foreach (Transform child in parent.transform)
            {
                var spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) continue;
                spriteRenderer.sortingOrder += value;
            }
        }
    }
}
