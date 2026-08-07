using Assets.Scripts.Actions;
using Assets.Scripts.Actions.Projectile;
using GameplayActions;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.GameplayActions.FireProjectile
{
    public class CommonProjectile : ProjectileInstance
    {
        [SerializeField] private TrailRenderer _flameTrail;
        [SerializeField] private TrailRenderer _residualTrail;

        public override void Setup(InteractionContext interactionContext, ProjectileData data, Action onDeactivate, Vector2 targetPosition)
        {
            SetTrailsEmitting(false);
            base.Setup(interactionContext, data, onDeactivate, targetPosition);

            ClearTrails();
            SetTrailsEmitting(true);
        }

        public override void Explode()
        {
            if (isExploded) return;
            isExploded = true;

            ExecuteExplosionActions();
            StartCoroutine(FadeTrailsAndReleaseRoutine());
        }

        private IEnumerator FadeTrailsAndReleaseRoutine()
        {
            SetTrailsEmitting(false);
            float waitTime = GetMaxTrailLifetime();
            if (waitTime > 0) yield return new WaitForSeconds(waitTime);

            ReleaseToPool();
        }

        private float GetMaxTrailLifetime()
        {
            float t1 = _flameTrail != null ? _flameTrail.time : 0f;
            float t2 = _residualTrail != null ? _residualTrail.time : 0f;
            return Mathf.Max(t1, t2);
        }

        private void ClearTrails()
        {
            if (_flameTrail != null) _flameTrail.Clear();
            if (_residualTrail != null) _residualTrail.Clear();
        }

        private void SetTrailsEmitting(bool emit)
        {
            if (_flameTrail != null) _flameTrail.emitting = emit;
            if (_residualTrail != null) _residualTrail.emitting = emit;
        }
    }
}