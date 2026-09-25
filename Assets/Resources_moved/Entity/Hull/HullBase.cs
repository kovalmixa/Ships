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
        #region Fields & Properties

        [Header("Data & Configuration")]
        [field: SerializeField] public HullDataSO Data { get; private set; }

        [Header("Physics & Movement State")]
        [HideInInspector] public float currentSpeed;
        [HideInInspector] public Vector2 externalVelocity;
        public Vector2 TotalVelocity => (Vector2)transform.up * currentSpeed + externalVelocity;

        [Header("Equipment & Hierarchy")]
        [HideInInspector] public Transform root;
        [HideInInspector] public List<EquipmentAnchor> equipmentAnchors = new();
        [HideInInspector] public List<Equipment.Equipment> equipments = new();

        protected Rigidbody2D rigidBody2D;
        protected override StatOptions StatOptions => Data != null ? Data.statOptions : default;
        protected override StatLayer StatLayer => StatLayer.Hull;

        public event Action OnMovement;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            rigidBody2D = GetComponent<Rigidbody2D>();
        }

        protected virtual void Update()
        {
            DampExternalVelocity();
            InvokeMovement();
        }

        #endregion

        #region Setup & Initialization

        public override void Setup(EntityController entityController)
        {
            base.Setup(entityController);

            abilitiesController = new AbilitiesController(
                StatOptions.abilities,
                entityController.TotalAbbilitiesController,
                actionDataController,
                this
            );

            OnGameObjectDestroyed += () => abilitiesController?.RemoveAbilities();

            CollectAnchors(transform);
        }

        private void CollectAnchors(Transform parent)
        {
            if (parent == null) return;
            foreach (Transform child in parent)
            {
                if (child.TryGetComponent<EquipmentAnchor>(out var equipmentAnchor))
                    equipmentAnchors.Add(equipmentAnchor);

                CollectAnchors(child);
            }
        }

        #endregion

        #region Movement & Physics

        public abstract void AddSpeed(bool isAddition);
        public abstract void SetTargetSpeed(Vector2 directionToPoint);
        public abstract void Movement(float rotationDirection);

        public void RotateEquipment(Vector3 target)
        {
            if (equipments == null) return;
            foreach (var eq in equipments)
                eq?.Rotate(target);
        }

        public void AddExternalForce(Vector2 force) => externalVelocity += force;

        private void DampExternalVelocity()
        {
            if (externalVelocity.sqrMagnitude > 0.001f)
            {
                externalVelocity = Vector2.MoveTowards(
                    externalVelocity,
                    Vector2.zero,
                    GetLifetimeStat(StatType.Mass) * 2f * Time.deltaTime
                );
                InvokeMovement();
            }
            else externalVelocity = Vector2.zero;
        }

        #endregion

        #region Collision & Triggers

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Bounce(collision);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IScript>(out var script))
                script.Execute(entityController);
        }

        private void Bounce(Collision2D collision)
        {
            Rigidbody2D otherRb = collision.rigidbody;
            if (otherRb == null) return;

            float otherMass = 1f;
            float hostMass = GetLifetimeStat(StatType.Mass);

            if (otherRb.TryGetComponent<HullBase>(out var otherHull))
                otherMass = otherHull.GetLifetimeStat(StatType.Mass);

            Rigidbody2D rb = rigidBody2D != null ? rigidBody2D : GetComponent<Rigidbody2D>();
            if (rb == null) return;

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

        #region Interface Implementations & Helpers

        public override IDataContainer GetInitialData() => Data;
        protected void InvokeMovement() => OnMovement?.Invoke();

        #endregion
    }
}