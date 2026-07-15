using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using Entity.Controllers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class TotalAbbilitiesController : MonoBehaviour
    {
        private EntityController _entityController;

        private readonly Dictionary<AbilityType, Action<InteractionContext, Vector3>> _equipmentAbilities = new();
        private readonly Dictionary<AbilityType, Action<InteractionContext, Vector3>> _entityAbilities = new();
        private bool _dirty = true;

        private void Awake()
        {
            _entityController = GetComponent<EntityController>();
        }

        public void MarkDirty() => _dirty = true;

        private void RebuildIfNeeded()
        {
            if (!_dirty) return;
            _equipmentAbilities.Clear();

            var hull = _entityController != null ? _entityController.hull : null;
            if (hull != null)
            {
                foreach (var equipment in hull.equipments)
                {
                    if (equipment == null) continue;
                    foreach (var ability in equipment.RuntimeAbilities)
                    {
                        if (ability == null) continue;
                        if (!_equipmentAbilities.ContainsKey(ability.type))
                            _equipmentAbilities[ability.type] = null;

                        _equipmentAbilities[ability.type] += (context, targetPos) =>
                        {
                            equipment.Activate(targetPos, ability, context);
                        };
                    }
                }
            }
            _dirty = false;
        }

        public void Invoke(Vector3 targetPos, AbilityType key)
        {
            if (key == AbilityType.None) return;
            if (IsAttackAbility(key) && IsPositionBlocked(targetPos)) return;
            RebuildIfNeeded();
            var context = CreateInteractionContext(key);
            if (_equipmentAbilities.TryGetValue(key, out var equipmentAction)) equipmentAction?.Invoke(context, targetPos);
            if (_entityAbilities.TryGetValue(key, out var entityAction)) entityAction?.Invoke(context, targetPos);
        }

        public void RegisterEntityAbility(AbilityType key, Action<InteractionContext, Vector3> action)
        {
            if (key == AbilityType.None || action == null) return;

            if (!_entityAbilities.ContainsKey(key)) _entityAbilities[key] = null;
            _entityAbilities[key] += action;
        }

        public void UnregisterEntityAbility(AbilityType key, Action<InteractionContext, Vector3> action)
        {
            if (_entityAbilities.ContainsKey(key)) _entityAbilities[key] -= action;
        }

        private static bool IsAttackAbility(AbilityType type) => type switch
        {
            AbilityType.FirePrimary => true,
            AbilityType.FireSecondary => true,
            AbilityType.LaunchAircraft => true,
            AbilityType.LaunchMissile => true,
            _ => false
        };

        private bool IsPositionBlocked(Vector3 position)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(position)) return true;

            var hull = _entityController != null ? _entityController.hull : null;
            if (hull == null) return false;

            foreach (var equipment in hull.equipments)
            {
                if (equipment == null) continue;
                col = equipment.GetComponent<Collider2D>();
                if (col != null && col.OverlapPoint(position)) return true;
            }
            return false;
        }

        private InteractionContext CreateInteractionContext(AbilityType key)
        {
            return new InteractionContext
            {
                SourceObject = gameObject,
                SourceSnapshot = _entityController != null ? _entityController.GetSnapshot() : null,
                AbilityId = key.ToString()
            };
        }
    }
}