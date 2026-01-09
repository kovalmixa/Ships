using System.Collections;
using Assets.Handlers.SceneHandlers;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Handlers.SceneHandlers;

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
        [SerializeField] private List<ParticleEffectData> _particles;
        [SerializeField] private List<SoundEffectData> _sounds;
        [SerializeField] private Animator _animator;

        [SerializeField] private bool _isLoopedAnimation;
        [SerializeField] private bool _isLoopedSound;

        private bool _isPlayedSound;
        private string _layerStateName = " ";
        private ObjectPoolHandler _objectPool;
        private AudioSourcePool _audioSourcePool;
        private AudioSource _currentAudioSource;

        public float Duration;
        public float Volume;
        private void Start()
        {
            _objectPool = SceneNodesHandler.GetPoolHandler("EffectPool");
            _audioSourcePool = SceneNodesHandler.GetPoolHandler("AudioSourcePool") as AudioSourcePool;
            _layerStateName = GetHitLayerName();
            Play();
        }

        private string GetHitLayerName()
        {
            Collider2D hitCollider = Physics2D.OverlapPoint(transform.position);
            int layer = 0;
            if (hitCollider != null)
            {
                GameObject hitObject = hitCollider.gameObject;
                layer = hitObject.layer;
            }
            return LayerMask.LayerToName(layer);
        }

        public void Play()
        {
            //Debug.Log($"Effect: {name}, layer {_layerStateName}");
            if (_animator == null)
            {
                Debug.LogError("_animator is null and cant play");
                return;
            }

            if (_animator.HasState(0, Animator.StringToHash(_layerStateName)))
                _animator?.Play(_layerStateName);
            //Debug.Log(GetAnimationLength(_animator));
            StartCoroutine(DestroyAfterEffect());
        }

        private IEnumerator DestroyAfterEffect()
        {
            float animDuration = Duration > 0 ? Duration : GetAnimationLength();
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

            if (_objectPool != null)
                _objectPool.Return(gameObject);
            else
                gameObject.SetActive(false);
        }

        protected float GetAnimationLength()
        {
            if (_animator == null 
                || !_animator.HasState(0, Animator.StringToHash(_layerStateName))) return 0f;
            _animator.Play(_layerStateName, 0, 0f);
            _animator.Update(0f);
            var info = _animator.GetCurrentAnimatorStateInfo(0);
            return info.length;
        }

        #region animation events

        public void PlaySound()
        {
            if (!_isLoopedSound && _isPlayedSound) return;
            var soundData = _sounds.FirstOrDefault(x => x.LayerName == _layerStateName);
            if (soundData?.AudioSource == null || soundData.AudioSource.clip == null) return;
            _currentAudioSource = soundData.AudioSource;
            _audioSourcePool.PlaySound(_currentAudioSource.clip, transform.position, Volume);
            _isPlayedSound = true;
        }

        public void PlayParticles()
        {
            var particle = _particles.First(x => x.LayerName == _layerStateName);
            if (!particle?.ParticleSystem == null) return;
            particle.ParticleSystem.Play();
        }
        #endregion
    }
}
