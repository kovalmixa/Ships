using Assets.Entity;
using Assets.Entity.Hull;
using Entity.Controllers.GenericController;
using UnityEngine;

namespace Assets.Handlers.SceneHandlers
{
    public class GameObjectHandler : SingletonMonoBehaviour<GameObjectHandler>
    {
        public static void SetRenderLayerOrder(GameObject parent, int value)
        {
            var renderers = parent.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in renderers)
            {
                spriteRenderer.sortingOrder += value;
            }
        }

        public static void Clone(GameObject main, GameObject obj)
        {
            if (main == null || obj == null) return;
            {
                foreach (var component in main.GetComponents<Component>())
                {
                    if (component is Transform) continue;
                    UnityEditorInternal.ComponentUtility.CopyComponent(component);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(obj);
                }
            }
        }

        public static void ClearComponents(GameObject obj)
        {
            if (obj == null) return;
            foreach (var component in obj.GetComponents<Component>())
            {
                if (component is Transform) continue;
                Destroy(component);
            }
        }

        public static EntityController GetEntityController(Collider2D other)
        {
            var hull = other.GetComponent<HullBase>();
            if (hull == null) return null;
            return hull.root.GetComponent<EntityController>();
        } 
    }
}
