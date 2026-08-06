using Assets.Entity;
using Assets.Entity.Controllers.AI;
using Assets.Entity.Hull;
using Assets.Scripts.Actions;
using Entity.Controllers;
using System;
using System.Reflection;
using UnityEngine;

namespace Assets.Handlers.SceneHandlers
{
    public class GameObjectHandler : SingletonMonoBehaviour<GameObjectHandler>
    {
        public static EntityController playerController;
        
        public static void SetRenderLayerOrder(GameObject parent, int value)
        {
            var renderers = parent.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in renderers) spriteRenderer.sortingOrder += value;
        }

        public static GameObject Clone(GameObject main)
        {
            if (main == null) return null;
            GameObject clone = Instantiate(main);
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
                Destroy(component);
            }
        }

        #region Entity controller

        public static EntityController GetEntityController(Collider2D other)
        {
            var hull = other.GetComponent<HullBase>();
            if (hull == null) return null;
            return hull.root.GetComponent<EntityController>();
        }

        public static IAIDriver GetAI(EntityController entityController)
        {
            if (entityController == null) return null;
            return entityController.TryGetComponent<IAIDriver>(out var ai) ? ai : null;
        }

        public static bool IsPlayer(EntityController entityController) => GetAI(entityController) == null;

        #endregion

        #region Id generators

        public static string GenerateUniqueId(string name) => $"{name}_{System.Guid.NewGuid().ToString("N").Substring(0, 8)}";

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
