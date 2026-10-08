using Assets.Handlers;
using Assets.Handlers.Enums;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Actions.Corpse
{
    public class CorpseInstance : MonoBehaviour
    {
        [SerializeField] private float _defaultDisappearTime = 300f;
        [SerializeField] private float _fadeOutDuration = 2f;

        private readonly List<SpriteRenderer> _renderers = new();
        private int _activeCount;

        private Action _onRelease;
        private CancellationTokenSource _cts;

        public void Setup(Transform source, SortingLayerType layer, Sprite hullSpriteOverride,
            Action onRelease, float disappearTime = -1f)
        {

            _onRelease = onRelease;
            transform.SetPositionAndRotation(source.position, source.rotation);
            transform.localScale = Vector3.one;

            var sources = source.GetComponentsInChildren<SpriteRenderer>(false);
            _activeCount = 0;

            foreach (var src in sources)
            {
                if (src == null || !src.enabled || src.sprite == null) continue;

                var sprite = hullSpriteOverride != null ? hullSpriteOverride : src.sprite;
                var dst = GetRenderer(_activeCount++);

                var t = dst.transform;
                t.SetPositionAndRotation(src.transform.position, src.transform.rotation);
                t.localScale = src.transform.lossyScale;

                dst.sprite = sprite;
                dst.flipX = src.flipX;
                dst.flipY = src.flipY;
                dst.sharedMaterial = src.sharedMaterial;
                dst.color = Color.white;

                LayersHandler.SetSortingLayer(dst, layer);
                dst.sortingOrder = src.sortingOrder;
            }

            for (int i = _activeCount; i < _renderers.Count; i++)
                _renderers[i].gameObject.SetActive(false);

            CancelLifetimeTask();
            _cts = new CancellationTokenSource();
            float wait = disappearTime > 0f ? disappearTime : _defaultDisappearTime;
            StartLifetimeSequenceAsync(wait, _cts.Token).Forget();
        }

        private SpriteRenderer GetRenderer(int index)
        {
            if (index < _renderers.Count)
            {
                var existing = _renderers[index];
                existing.gameObject.SetActive(true);
                return existing;
            }

            var go = new GameObject($"Part_{index}", typeof(SpriteRenderer));
            go.transform.SetParent(transform, false);
            var sr = go.GetComponent<SpriteRenderer>();
            _renderers.Add(sr);
            return sr;
        }

        private async UniTaskVoid StartLifetimeSequenceAsync(float disappearTime, CancellationToken ct)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(disappearTime), cancellationToken: ct);

                float timer = 0f;
                while (timer < _fadeOutDuration)
                {
                    timer += Time.deltaTime;
                    float alpha = Mathf.Lerp(1f, 0f, timer / _fadeOutDuration);
                    for (int i = 0; i < _activeCount; i++)
                        _renderers[i].color = new Color(1f, 1f, 1f, alpha);

                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
                Release();
            }
            catch (OperationCanceledException) { }
        }

        public void Release()
        {
            CancelLifetimeTask();
            _onRelease?.Invoke();
            _onRelease = null;
        }

        private void CancelLifetimeTask()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        private void OnDisable() => CancelLifetimeTask();
    }
}