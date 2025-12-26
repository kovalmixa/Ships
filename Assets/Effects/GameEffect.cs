using Assets.Handlers;
using Assets.Handlers.SceneHandlers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Effects
{
    public class GameEffect : MonoBehaviour
    {
        [SerializeField] private string defaultState = "Default";
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private AudioSource[] audioSources; 
        [SerializeField] private float duration = 0; // Duration in seconds, 0 means play once
        [SerializeField] private Animator animator;
        private ObjectPoolHandler _objectPool;

        private void Start()
        {
            _objectPool = SceneNodesHandler.GetPoolHandler("EffectPool");
            Play();
        }
        
        public void Play()
        {
            Debug.Log($"Effect: {name}");
            Collider2D hitCollider = Physics2D.OverlapPoint(transform.position);
            int layer = 0;
            if (hitCollider != null)
            {
                GameObject hitObject = hitCollider.gameObject;
                layer = hitObject.layer;
            }
            string stateName = GetStateByLayer(layer);
            Debug.Log($"playing with state: {stateName}");

            animator?.Play(stateName);

            var audioSource = FunctionHandler.GetRandomElementArray(audioSources);

            if (particle != null)
                particle.Play();

            if (audioSource != null)
                audioSource.Play();

            StartCoroutine(DestroyAfterEffect(animator, stateName, audioSource));
        }

        private string GetStateByLayer(int layer)
        {
            string layerName = LayerMask.LayerToName(layer);

            if (animator.HasState(0, Animator.StringToHash(layerName)))
                return layerName;

            return defaultState;
        }

        private System.Collections.IEnumerator DestroyAfterEffect(Animator animator, string stateName, AudioSource audioSource)
        {
            float animDuration = GetAnimationLength(animator, stateName);
            float soundDuration = audioSource != null ? audioSource.clip.length : 0f;

            float delay = Mathf.Max(animDuration, soundDuration);
            yield return new WaitForSeconds(delay);

            if (_objectPool != null) _objectPool.Return(gameObject);
            else gameObject.SetActive(false);
        }

        protected float GetAnimationLength(Animator animator, string animationName)
        {
            if (animator == null || animator.runtimeAnimatorController == null) return 0f;
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == animationName)
                    return clip.length;
            }
            return 0f;
        }
    }
}
