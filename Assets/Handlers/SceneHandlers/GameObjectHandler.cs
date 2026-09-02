using Assets.AI;
using Assets.Entity.Hull;
using Assets.Scripts.Actions;
using Entity.Controllers;
using System;
using System.Reflection;
using UnityEngine;

namespace Assets.Handlers.SceneHandlers
{
    public static class GameObjectHandler
    {
        #region Monobehavior nodes

        #region Layers

        public static void SetRenderLayerOrder(GameObject parent, string layerName, int orderOffset)
        {
            var renderers = parent.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var spriteRenderer in renderers)
            {
                spriteRenderer.sortingLayerName = layerName;
                spriteRenderer.sortingOrder += orderOffset;
            }
        }

        #endregion

        #region Scene Search Helpers

        public static GameObject GetNodeByName(string name)
        {
            GameObject node = GameObject.Find(name);
            if (node == null)
            {
                Transform dontDestroy = GameObject.Find("DontDestroyOnLoad")?.transform;
                if (dontDestroy != null)
                {
                    Transform found = dontDestroy.Find(name);
                    if (found != null) return found.gameObject;
                }
                return null;
            }
            return node;
        }

        public static T GetNodeByType<T>() where T : Component
        {
            T node = GameObject.FindAnyObjectByType<T>(FindObjectsInactive.Include);
            if (node == null)
            {
                Transform dontDestroy = GameObject.Find("DontDestroyOnLoad")?.transform;
                if (dontDestroy != null) node = dontDestroy.GetComponentInChildren<T>(true);
            }

            return node;
        }

        #endregion

        #region Copy/Clone/Clearing Components

        public static GameObject Clone(GameObject main)
        {
            if (main == null) return null;
            GameObject clone = GameObject.Instantiate(main);
            clone.name = main.name;
            return clone;
        }


        public static void CopyComponentsTo(GameObject source, GameObject target)
        {
            Component[] components = source.GetComponents<Component>();
            foreach (Component sourceComp in components)
            {
                if (sourceComp is Transform) continue;
                Type type = sourceComp.GetType();
                Component targetComp = target.AddComponent(type);
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields) field.SetValue(targetComp, field.GetValue(sourceComp));
            }
        }

        public static void ClearComponents(GameObject obj)
        {
            if (obj == null) return;
            foreach (var component in obj.GetComponents<Component>())
            {
                if (component is Transform) continue;
                GameObject.Destroy(component);
            }
        }

        #endregion

        #endregion

        #region Entity controller

        public static EntityController GetEntityController(Collider2D other)
        {
            if (other == null) return null;
            var hull = other.GetComponent<HullBase>();
            if (hull == null || hull.root == null) return null;
            return hull.root.GetComponent<EntityController>();
        }

        public static IAiDriver GetAI(EntityController entityController)
        {
            if (entityController == null) return null;
            return entityController.TryGetComponent<IAiDriver>(out var ai) ? ai : null;
        }

        public static bool IsPlayer(EntityController entityController) => GetAI(entityController) == null;

        #endregion

        #region Id generators

        public static string GenerateUniqueId(string name) => $"{name}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

        public static string GenerateContextSourceId(InteractionContext context)
        {
            if (context == null) return "Unknown";
            if (!string.IsNullOrEmpty(context.AbilityId)) return context.AbilityId;
            if (context.SourceObject != null) return context.SourceObject.name;
            return context.SourceSnapshot?.Id ?? "System";
        }

        #endregion
    }
}
