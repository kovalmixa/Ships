using Assets.Common;
using Assets.Common.Interfaces;
using Assets.DataContainers;
using Assets.Entity.Common;
using Assets.Entity.Controllers;
using Assets.Entity.Equipment;
using Assets.Entity.Interfaces;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using Entity.Controllers;
using GameplayActions;
using Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Hull
{
    public abstract class HullBase : EntityPartBase, IHull
    {
        [field: SerializeField] public HullDataSO Data { get; private set; }

        [HideInInspector] public List<EquipmentAnchor> equipmentAnchors = new();
        [HideInInspector] public List<Equipment.Equipment> equipments = new();
        [HideInInspector] public Transform root;
        [HideInInspector] public float currentSpeed;

        protected Rigidbody2D rigidBody2D;

        protected override StatOptions StatOptions => Data.statOptions;
        protected override StatLayer StatLayer => StatLayer.Hull;
        public override IDataContainer GetInitialData() => Data;

        #region Setup

        protected override void Awake()
        {
            base.Awake();
            rigidBody2D = GetComponent<Rigidbody2D>();
        }

        public override void Setup(EntityController entityController)
        {
            base.Setup(entityController);

            abilitiesController = new AbilitiesController(
                StatOptions.abilities,
                entityController.TotalAbbilitiesController,
                _actionDataController,
                this
            );
            OnGameObjectDestroyed += () => abilitiesController.RemoveAbilities();

            CollectAnchors(transform);
        }

        private void CollectAnchors(Transform parent)
        {
            if (parent == null) return;
            foreach (Transform child in parent)
            {
                var equipmentAnchor = child.GetComponent<EquipmentAnchor>();
                if (equipmentAnchor != null) equipmentAnchors.Add(equipmentAnchor);
                CollectAnchors(child);
            }
        }

        #endregion

        #region Movement

        public void RotateEquipment(Vector3 target)
        {
            foreach (var eq in equipments) eq.Rotate(target);
        }

        public abstract void AddSpeed(bool isAddition);
        public abstract void SetTargetSpeed(Vector2 directionToPoint);
        public abstract void Movement(float rotationDirection);

        #endregion

        #region Triggers

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Bounce(collision);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            IScript script = other.GetComponent<IScript>();
            script?.Execute(entityController);
        }

        private void Bounce(Collision2D collision)
        {
            Rigidbody2D otherRb = collision.rigidbody;
            if (otherRb == null) return;

            if (collision.gameObject.layer != LayerMask.NameToLayer(Data.general.Layer.ToString())
                && collision.gameObject.layer != LayerMask.NameToLayer("Markers"))
            {
                currentSpeed = 0;
                return;
            }

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Vector2 pushDirection = (rb.position - otherRb.position).normalized;
            float totalMass = rb.mass + otherRb.mass;
            float impulse = currentSpeed * rb.mass * 0.1f;

            otherRb.AddForce(-pushDirection * (impulse * (rb.mass / totalMass)), ForceMode2D.Impulse);
            currentSpeed *= otherRb.mass / totalMass;
        }

        #endregion

        #region IAbbility Implementation

        #endregion
    }
}