using Actions;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using Entity.Controllers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public enum AbilityActivationMode
    {
        Primary,
        Ability,
    }

    [Serializable]
    public class ItemAbilities
    {
        public Vector2 Position;
        public AbilityType Ability;
        public AbilityActivationMode Mode;
    }

    public class AbbilitiesController : MonoBehaviour
    {
        private EntityController _entityController;
        private readonly Dictionary<AbilityType, Action> _equipmentAbilities = new();
        private readonly Dictionary<AbilityType, Action> _entityAbilities = new();
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
                        if (!_equipmentAbilities.TryGetValue(ability.Ability, out var list))
                            _equipmentAbilities[ability.Ability] = list = new ();
                        list.Add(equipment.Position);
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
            if (_equipmentAbilities.TryGetValue(key, out var positions))
                foreach (var startPos in positions)
                    ExecuteAction(key, startPos, targetPos);

            if (_entityAbilities.TryGetValue(key, out var entityAction))
                ExecuteAction(key);
        }

        public void RegisterEntityAbility(AbilityType key, Action<InterractionContext, Vector2> action)
        {
            if (key == AbilityType.None || action == null) return;
            _entityAbilities[key] = action;
        }

        public bool UnregisterEntityAbility(AbilityType key) => _entityAbilities.Remove(key);

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
                col = equipment.GetComponent<Collider2D>();
                if (col != null && col.OverlapPoint(position)) return true;
            }
            return false;
        }

        private void ExecuteAction(AbilityType key)
        {
            var context = new InterractionContext
            {
                SourceObject = gameObject,
                SourceSnapshot = _entityController != null ? _entityController.GetSnapshot() : null,
                AbilityId = key.ToString()
            };
            switch (key) {
                case AbilityType.None:
                {
                    break;
                }
                case AbilityType.FirePrimary:
                {
                    break;
                }
                case AbilityType.FireSecondary:
                {
                    break;
                }
            }
        }
    }
}
