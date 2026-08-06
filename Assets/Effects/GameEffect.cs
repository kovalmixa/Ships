using System.Collections;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Effects
{
    #region Daos
    [System.Serializable]
    public class ParticleEffectData
    {
        public string LayerName;
        public ParticleSystem ParticleSystem;
    }

    [System.Serializable]
    public class SoundEffectData
    {
        public string LayerName;
        public AudioSource AudioSource;
    }

    #endregion

    public class GameEffect : MonoBehaviour
    {
        private ParticleSystem[] _particles;
        private AudioSource _audioSource;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;

        [SerializeField] private bool _isLoopedAnimation;
        [SerializeField] private bool _isLoopedSound;
        public float duration;
        public float volume;

        private bool isPlayedSound;
        //private ObjectPoolHandler _effectsPool;

        //private AudioSourcePool _audioSourcePool;
        private AudioSource _currentAudioSource;

        private void Start()
        {
            //_effectsPool = SceneController.GetPoolHandler("EffectPool");
            //_audioSourcePool = SceneNodesHandler.GetPoolHandler("AudioSourcePool") as AudioSourcePool;
            _particles = gameObject.GetComponentsInChildren<ParticleSystem>();
            _audioSource = gameObject.GetComponentInChildren<AudioSource>();
            _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
            _animator = gameObject.GetComponentInChildren<Animator>();
            Play();
        }



        public void Play()
        {
            //Debug.Log($"Effect: {name}, layer {layerStateName}");
            if (_animator == null)
            {
                Debug.LogError("animator is null and cant play");
                return;
            }

            //Debug.Log(GetAnimationLength(animator));
            StartCoroutine(DestroyAfterEffect());
        }

        private IEnumerator DestroyAfterEffect()
        {
            float animDuration = duration > 0 ? duration : GetAnimationLength();
            float soundDuration = _currentAudioSource != null ? _currentAudioSource.clip.length : 0f;
            float totalWait = Mathf.Max(animDuration, soundDuration);
            // ∆дЄм всю длительность анимации
            yield return new WaitForSeconds(animDuration);
            if (!_isLoopedAnimation)
            {
                if (_animator != null) _animator.enabled = false;
                // ¬ажно! —брасываем спрайт вручную
                var spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null) spriteRenderer.sprite = null;
            }

            float remaining = totalWait - animDuration;
            if (remaining > 0.01f) yield return new WaitForSeconds(remaining);

            //if (_effectsPool != null)
            //    _effectsPool.Release(gameObject);
            //else
            //    gameObject.SetActive(false);
        }

        protected float GetAnimationLength()
        {
            //_animator.Play(layerStateName, 0, 0f);
            _animator.Update(0f);
            var info = _animator.GetCurrentAnimatorStateInfo(0);
            return info.length;
        }

        #region animation events

        public void PlaySound()
        {
            if (!_isLoopedSound && isPlayedSound) return;
            if (_audioSource == null || _audioSource.clip == null) return;
            //_audioSourcePool.PlaySound(_currentAudioSource.clip, transform.position, volume);
            isPlayedSound = true;
        }

        public void PlayParticles()
        {
            if (_particles == null || _particles.Length == 0) return;
            foreach (var particle in _particles) particle.Play();
        }
        #endregion
    }
}
