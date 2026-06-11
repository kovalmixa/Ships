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
        [SerializeField] private List<ParticleEffectData> particles;
        [SerializeField] private List<SoundEffectData> sounds;
        [SerializeField] private Animator animator;

        [SerializeField] private bool isLoopedAnimation;
        [SerializeField] private bool isLoopedSound;

        private bool isPlayedSound;
        private string layerStateName = " ";
        private ObjectPoolHandler objectPool;
        private AudioSourcePool audioSourcePool;
        private AudioSource currentAudioSource;

        public float Duration;
        public float Volume;
        private void Start()
        {
            objectPool = SceneNodesHandler.GetPoolHandler("EffectPool");
            audioSourcePool = SceneNodesHandler.GetPoolHandler("AudioSourcePool") as AudioSourcePool;
            layerStateName = GetHitLayerName();
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
            //Debug.Log($"Effect: {name}, layer {layerStateName}");
            if (animator == null)
            {
                Debug.LogError("animator is null and cant play");
                return;
            }

            if (animator.HasState(0, Animator.StringToHash(layerStateName)))
                animator?.Play(layerStateName);
            //Debug.Log(GetAnimationLength(animator));
            StartCoroutine(DestroyAfterEffect());
        }

        private IEnumerator DestroyAfterEffect()
        {
            float animDuration = Duration > 0 ? Duration : GetAnimationLength();
            float soundDuration = currentAudioSource != null ? currentAudioSource.clip.length : 0f;
            float totalWait = Mathf.Max(animDuration, soundDuration);
            // ∆дЄм всю длительность анимации
            yield return new WaitForSeconds(animDuration);
            if (!isLoopedAnimation)
            {
                if (animator != null) animator.enabled = false;
                // ¬ажно! —брасываем спрайт вручную
                var spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null) spriteRenderer.sprite = null;
            }

            float remaining = totalWait - animDuration;
            if (remaining > 0.01f) yield return new WaitForSeconds(remaining);

            if (objectPool != null)
                objectPool.Return(gameObject);
            else
                gameObject.SetActive(false);
        }

        protected float GetAnimationLength()
        {
            if (animator == null 
                || !animator.HasState(0, Animator.StringToHash(layerStateName))) return 0f;
            animator.Play(layerStateName, 0, 0f);
            animator.Update(0f);
            var info = animator.GetCurrentAnimatorStateInfo(0);
            return info.length;
        }

        #region animation events

        public void PlaySound()
        {
            if (!isLoopedSound && isPlayedSound) return;
            var soundData = sounds.FirstOrDefault(x => x.LayerName == layerStateName);
            if (soundData?.AudioSource == null || soundData.AudioSource.clip == null) return;
            currentAudioSource = soundData.AudioSource;
            audioSourcePool.PlaySound(currentAudioSource.clip, transform.position, Volume);
            isPlayedSound = true;
        }

        public void PlayParticles()
        {
            var particle = particles.First(x => x.LayerName == layerStateName);
            if (!particle?.ParticleSystem == null) return;
            particle.ParticleSystem.Play();
        }
        #endregion
    }
}
