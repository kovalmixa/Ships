using Assets.Handlers;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using Entity.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class TotalAbbilitiesController
    {
        private EntityController _entityController;

        private readonly Dictionary<AbilityType, Action<InteractionContext, Vector3>> _activeAbilities = new();
        private readonly Dictionary<WeaponType, Action<InteractionContext, Vector3>> _weaponAbilities = new();

        private readonly Dictionary<AbilityType, Action<InteractionContext, Vector3>> _entityAbilities = new();

        private bool _dirty = true;

        public TotalAbbilitiesController(EntityController entityController) => _entityController = entityController;

        public void MarkDirty() => _dirty = true;

        private void RebuildIfNeeded()
        {
            if (!_dirty) return;

            _activeAbilities.Clear();
            _weaponAbilities.Clear();

            var hull = _entityController != null ? _entityController.hull : null;
            if (hull != null && hull.equipments != null)
            {
                void RegisterAbility<TKey>(
                    Dictionary<TKey, Action<InteractionContext, Vector3>> dictionary,
                    TKey key,
                    Action<InteractionContext, Vector3> action)
                {
                    if (!dictionary.ContainsKey(key)) dictionary[key] = null;
                    dictionary[key] += action;
                }

                // 1. Hull abilities
                foreach (var ability in hull.RuntimeAbilities)
                {
                    RegisterAbility(_activeAbilities, ability.type,
                        (context, targetPos) => hull.Activate(targetPos, ability, context));
                }

                // 2. Group weapon abilities
                var weaponGroups = EquipmentHandler.GroupWeaponsByTier(hull.equipments);
                foreach (var kvp in weaponGroups)
                {
                    WeaponType weaponType = kvp.Key;
                    foreach (var equipment in kvp.Value)
                    {
                        if (equipment == null) continue;
                        var groupAbilities = equipment.RuntimeAbilities
                            .Where(a => a.mode == AbilityActivationMode.WeaponGroup);

                        foreach (var ability in groupAbilities)
                        {
                            var currentEquipment = equipment;
                            var currentAbility = ability;

                            RegisterAbility(_weaponAbilities, weaponType,
                                (context, targetPos) => currentEquipment.Activate(targetPos, currentAbility, context));
                        }
                    }
                }

                // 3. Active equipment abilities
                foreach (var equipment in hull.equipments)
                {
                    if (equipment == null) continue;
                    var activeAbilities = equipment.RuntimeAbilities
                        .Where(a => a.mode == AbilityActivationMode.ActiveAbility);

                    foreach (var ability in activeAbilities)
                    {
                        var currentEquipment = equipment;
                        var currentAbility = ability;
                        RegisterAbility(_activeAbilities, ability.type,
                            (context, targetPos) => currentEquipment.Activate(targetPos, currentAbility, context));
                    }
                }
            }
            _dirty = false;
        }

        public void Invoke(Vector3 targetPos, WeaponType weaponType)
        {
            if (weaponType == WeaponType.None) return;
            if (IsPositionBlocked(targetPos)) return;

            RebuildIfNeeded();

            if (_weaponAbilities.TryGetValue(weaponType, out var weaponAction))
            {
                var context = CreateInteractionContext(AbilityType.FireWeapon);
                weaponAction?.Invoke(context, targetPos);
            }
        }

        public void Invoke(Vector3 targetPos, AbilityType abilityType)
        {
            if (abilityType == AbilityType.None) return;
            if (IsAttackAbility(abilityType) && IsPositionBlocked(targetPos)) return;

            RebuildIfNeeded();
            var context = CreateInteractionContext(abilityType);

            if (_activeAbilities.TryGetValue(abilityType, out var equipAction))
                equipAction?.Invoke(context, targetPos);

            if (_entityAbilities.TryGetValue(abilityType, out var entityAction))
                entityAction?.Invoke(context, targetPos);
        }

        public void RegisterEntityAbility(AbilityType key, Action<InteractionContext, Vector3> action)
        {
            if (key == AbilityType.None || action == null) return;
            if (!_entityAbilities.ContainsKey(key)) _entityAbilities[key] = null;
            _entityAbilities[key] += action;
        }

        public void UnregisterEntityAbility(AbilityType key, Action<InteractionContext, Vector3> action)
            { if (_entityAbilities.ContainsKey(key)) _entityAbilities[key] -= action; }

        private static bool IsAttackAbility(AbilityType type) => type switch
        {
            AbilityType.FireWeapon => true,
            AbilityType.LaunchAircraft => true,
            AbilityType.LaunchMissile => true,
            AbilityType.LaunchTorpedo => true,
            AbilityType.DropBomb => true,
            AbilityType.FireLaser => true,
            _ => false
        };

        private bool IsPositionBlocked(Vector3 position)
        {
            Collider2D hitCollider = Physics2D.OverlapPoint(position);
            if (hitCollider == null) return false;

            if (_entityController != null && _entityController.hull != null)
            {
                if (hitCollider.gameObject == _entityController.hull.gameObject) return false;
                if (_entityController.hull.equipments != null)
                    foreach (var equipment in _entityController.hull.equipments)
                        if (equipment != null && hitCollider.gameObject == equipment.gameObject) return false;
            }
            return true;
        }

        private InteractionContext CreateInteractionContext(AbilityType key)
        {
            var context = new InteractionContext();
            context.SourceSnapshot = _entityController != null ? _entityController.GetSnapshot() : null;
            context.AbilityId = key.ToString();

            if (_entityController != null) context.SetSource(_entityController.gameObject);

            return context;
        }
    }
}