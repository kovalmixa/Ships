using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Actions.Decals
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DecalInstance : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float _defaultDisappearTime = 300f;
        [SerializeField] private float _fadeOutDuration = 2f;

        private Action _onRelease;
        private CancellationTokenSource _cts;
        private Color _originalColor;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            _originalColor = spriteRenderer.color;
        }

        public void Setup(Sprite sprite, Action onRelease, float disappearTime = -1f)
        {
            _onRelease = onRelease;
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = _originalColor;

            float timeToWait = disappearTime > 0f ? disappearTime : _defaultDisappearTime;
            CancelLifetimeTask();
            _cts = new CancellationTokenSource();

            StartLifetimeSequenceAsync(timeToWait, _cts.Token).Forget();
        }

        private async UniTaskVoid StartLifetimeSequenceAsync(float disappearTime, CancellationToken ct)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(disappearTime), cancellationToken: ct);

                float timer = 0f;
                Color startColor = spriteRenderer.color;

                while (timer < _fadeOutDuration)
                {
                    timer += Time.deltaTime;
                    float alpha = Mathf.Lerp(startColor.a, 0f, timer / _fadeOutDuration);
                    spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
                Release();
            }
            catch (OperationCanceledException)
            {
                // Игнорируем при отмене
            }
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