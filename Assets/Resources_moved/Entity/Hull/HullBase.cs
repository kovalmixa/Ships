using Assets.Common;
using Assets.Common.Interfaces;
using Assets.DataContainers;
using Assets.Entity.Common;
using Assets.Entity.Controllers;
using Assets.Entity.Equipment;
using Assets.Entity.Modifiers;
using Entity.Controllers;
using Scripts;
using System;
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
        [HideInInspector] public Vector2 externalVelocity;
        public Vector2 TotalVelocity => (Vector2)transform.up * currentSpeed + externalVelocity;

        public event Action OnMovement;
        protected Rigidbody2D rigidBody2D;

        protected override StatOptions StatOptions => Data.statOptions;
        protected override StatLayer StatLayer => StatLayer.Hull;
        public override IDataContainer GetInitialData() => Data;

        #region Life Cycle

        protected override void Awake()
        {
            base.Awake();
            rigidBody2D = GetComponent<Rigidbody2D>();
        }

        #region Setup

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

        #region Update

        protected virtual void Update()
        {
            DampExternalVelocity();
            InvokeMovement();
        }

        private void DampExternalVelocity()
        {
            if (externalVelocity.sqrMagnitude > 0.001f)
            {
                externalVelocity = Vector2.MoveTowards(
                    externalVelocity,
                    Vector2.zero,
                    GetLifetimeStat(StatType.Mass) * 2 * Time.deltaTime
                );
                InvokeMovement();
            }
            else externalVelocity = Vector2.zero;
        }

        #endregion

        #endregion

        #region Movement

        public void RotateEquipment(Vector3 target)
        {
            foreach (var eq in equipments) eq.Rotate(target);
        }

        public abstract void AddSpeed(bool isAddition);
        public abstract void SetTargetSpeed(Vector2 directionToPoint);
        public abstract void Movement(float rotationDirection);

        public void AddExternalForce(Vector2 force) => externalVelocity += force;

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

            float otherMass = 1, hostMass = GetLifetimeStat(StatType.Mass);
            if (otherRb.TryGetComponent(out HullBase otherHull))
                otherMass = otherHull.GetLifetimeStat(StatType.Mass);

            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            Vector2 pushDirection = (rb.position - otherRb.position).normalized;
            Vector2 hostDirection = rb.linearVelocity.sqrMagnitude > 0.01f ? rb.linearVelocity.normalized : (Vector2)transform.up;
            Vector2 otherDirection = otherRb.linearVelocity.sqrMagnitude > 0.01f ? otherRb.linearVelocity.normalized : (Vector2)otherRb.transform.up;

            float dotProduct = Vector2.Dot(hostDirection, otherDirection);
            float angleImpactFactor = Mathf.Clamp01((1f - dotProduct) / 2f);
            float totalMass = hostMass + otherMass;
            float impulseMultiplier = 0.5f + angleImpactFactor;
            float impulse = currentSpeed * hostMass * 0.1f * impulseMultiplier;

            Vector2 externalImpactForce = -pushDirection * (impulse * (hostMass / totalMass));

            if (otherHull != null) otherHull.AddExternalForce(externalImpactForce);
            else otherRb.AddForce(externalImpactForce, ForceMode2D.Impulse);

            AddExternalForce(pushDirection * (impulse * (otherMass / totalMass)));

            float speedTransferRatio = (otherMass / totalMass) * (1f - (angleImpactFactor * 0.5f));
            currentSpeed *= Mathf.Clamp(speedTransferRatio, 0.1f, 1f);

            InvokeMovement();
        }

        #endregion

        #region IAbbility Implementation

        #endregion

        #region Event invokations

        protected void InvokeMovement() => OnMovement?.Invoke();

        #endregion
    }
}