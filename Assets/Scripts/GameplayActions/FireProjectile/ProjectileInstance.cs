using Assets.Common;
using Assets.Common.Interfaces;
using Assets.Handlers;
using Assets.Handlers.Enums;
using Assets.Handlers.FileHandlers;
using Assets.Scripts.Actions.VFX;
using Cysharp.Threading.Tasks;
using GameplayActions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Assets.Scripts.Actions.Projectile
{
    public class ProjectileInstance : MonoBehaviour, IPoolInstance
    {
        protected readonly GameplayAction[] onExplosionActions;
        protected Action onReturnToPool;

        protected ProjectileData data;
        protected InteractionContext context;

        [SerializeField] protected float avarageDamage; //for setting size depending on damage value
        [SerializeField] protected VfxData launchEffectData;
        [SerializeField] protected VfxData explosionEffectData;
        [SerializeField] protected VfxRotationType vfxRotation = VfxRotationType.Default;
        [SerializeField] protected VfxType defaultExpVfx;

        protected Transform targetTransform;
        protected Vector2 targetPosition;
        protected Vector2 direction;


        protected float timer;
        protected bool isReturned;

        #region Private/Default

        public void Tick(float deltaTime)
        {
            Move(deltaTime);
            CheckLifetime(deltaTime);
        }

        private void CheckLifetime(float deltaTime)
        {
            if (data.lifeTime != 0)
            {
                timer += deltaTime;
                if (timer >= data.lifeTime) TryExplode();
            }
        }

        private void IgnoreShooterCollision(bool ignore)
        {
            if (context?.SourceObject == null) return;

            var projectileCollider = GetComponent<Collider2D>();
            var shooterCollider = context.SourceObject.GetComponent<Collider2D>();

            if (projectileCollider != null && shooterCollider != null)
                Physics2D.IgnoreCollision(projectileCollider, shooterCollider, ignore);
        }

        #endregion

        #region Setup

        public virtual void Setup(InteractionContext context, ProjectileData data, Action onDeactivate,Transform targetTransform)
        {
            this.targetTransform = targetTransform;
            Setup(context, data, onDeactivate, targetTransform.position);
        }

        public virtual void Setup(InteractionContext interactionContext, ProjectileData data, Action onReturnToPool, Vector2 targetPosition)
        {
            context = interactionContext;
            this.data = data;
            timer = 0f;
            this.targetPosition = targetPosition;
            direction = (this.targetPosition - this.data.startPosition).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.SetPositionAndRotation(this.data.startPosition, Quaternion.Euler(0, 0, angle - 90f));

            IgnoreShooterCollision(true);
            SetLaunchEffect();

            isReturned = false;
            this.onReturnToPool = onReturnToPool;
            gameObject.SetActive(true);
        }

        protected virtual void SetLaunchEffect() => ActionProvider.Vfx.Execute(context, launchEffectData, data.startPosition);

        #endregion

        #region Public/Protected

        protected virtual void Move(float deltaTime)
        {
            if (isReturned) return;
            if (data.isHoming && targetTransform != null)
            {
                Vector2 toTarget = (targetTransform.position - transform.position).normalized;
                direction = Vector2.Lerp(direction, toTarget, deltaTime * 5f);

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }

            float step = data.speed * deltaTime;
            float distToTarget = Vector2.Distance(transform.position, targetPosition);
            transform.position += (Vector3)(direction * step);
            if (distToTarget <= step || distToTarget <= 0.2f) TryExplode();
        }

        #region Explosion

        public virtual bool TryExplode(bool isContinuous = false)
        {
            if (isReturned) return false;
            isReturned = !isContinuous;

            ExecuteExplosionAction();
            if (onExplosionActions != null)
            {
                foreach (var action in onExplosionActions)
                {
                    var data = context.ActionDataController.GetActionData(action.GetType(), context, true);
                    action.Execute(context, data, transform.position);
                }
            }
            if (!isContinuous) ReleaseToPool();
            return true;
        }

        protected async UniTask ExecuteExplosionAction()
        {
            if (data?.damageData == null) { Debug.LogError("Damage is null"); return; }
            Vector3 explodePos = transform.position;
            var expData = new ExplosionData {
                vfxData = new VfxData { rotationType = vfxRotation },
                damageData = data.damageData
            };

            int ignoreMask = 0;
            if (LayersHandler.interactionIgnore != null)
            {
                foreach (var layerNameItem in LayersHandler.interactionIgnore)
                {
                    int layerIndex = LayerMask.NameToLayer(layerNameItem);
                    if (layerIndex != -1) ignoreMask |= (1 << layerIndex);
                }
            }

            Collider2D[] hitColliders = Physics2D.OverlapPointAll(explodePos, ~ignoreMask);
            string layerName = "";
            if (hitColliders.Length > 0)
            {
                foreach (var col in hitColliders)
                {
                    string currentLayer = LayerMask.LayerToName(col.gameObject.layer);
                    if (currentLayer == "Land")
                    {
                        layerName = "Land";
                        break;
                    }
                    if (currentLayer == "Water" && layerName != "Land") layerName = "Water";
                    else if (string.IsNullOrEmpty(layerName)) layerName = currentLayer;
                }
            }

            var targetLayer = data.damageData.targetLayer;
            var range = data.damageData.range;
            var airLayerMask = 1 << LayerMask.NameToLayer("Air");

            Collider2D[] targetCollider = Physics2D.OverlapCircleAll(explodePos, range, ~ignoreMask);
            bool hitInteractiveTarget = targetCollider.Any(col => col.GetComponent<IInteractive>() != null);
            bool isAirUnitNearSplash = Physics2D.OverlapCircle(explodePos, range * 3f, airLayerMask) != null;

            if (hitInteractiveTarget) layerName = "";
            else if (((targetLayer & LayerType.Air) != 0 || targetLayer == LayerType.All) && isAirUnitNearSplash) layerName = "Air";
            else if (string.IsNullOrEmpty(layerName)) layerName = "Air";
            else if (string.IsNullOrEmpty(layerName)) layerName = "Air";

            string layerVfx = $"{data.type}Hit{layerName}";
            if (Enum.TryParse(layerVfx, out VfxType result) && await VfxController.Instance.IsExist(result)) expData.vfxData.type = result;
            else if (await VfxController.Instance.IsExist(defaultExpVfx)) expData.vfxData.type = defaultExpVfx;
            else { Debug.LogWarning($"There is no match for both layer vfx({layerVfx}) and default one({defaultExpVfx})"); return; }
            ActionProvider.Explosion.Execute(context, expData, explodePos);
        }

        public void ReleaseToPool()
        {
            onReturnToPool?.Invoke();
            onReturnToPool = null;
        }

        #endregion

        #endregion
    }
}