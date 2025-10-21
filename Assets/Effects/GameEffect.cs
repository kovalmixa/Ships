using Assets.Handlers;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Assets.Effects
{
    public class GameEffect : MonoBehaviour
    {
        [SerializeField] protected ParticleSystem Particle;
        [SerializeField] protected AudioSource[] AudioSources;
        protected Animator Animator;
        private ObjectPoolHandler _objectPool;

        private void Start()
        {
            _objectPool = SceneNodesHandler.GetPoolHandler("EffectPool");
        }
        public void SetupByPrefab(GameEffect prefab)
        {
            Particle = prefab.Particle;
            AudioSources = prefab.AudioSources;
            Animator = prefab.Animator;
        }

        public void Play()
        {
            Debug.Log("just effect");
            var randomClipName = PlayRandomAnimation();
            var audioSource = FunctionHandler.GetRandomElementArray(AudioSources);
            if (Particle != null)
                Particle.Play();

            if (audioSource != null)
                audioSource.Play();

            StartCoroutine(DestroyAfterEffect(Animator, randomClipName, audioSource));
        }

        private System.Collections.IEnumerator DestroyAfterEffect(Animator animator, string clip_name, AudioSource audioSource)
        {
            float animDuration = GetAnimationLength(animator, clip_name);
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

        protected string PlayRandomAnimation()
        {
            string randomClipName = null;
            if (Animator != null && Animator.runtimeAnimatorController != null)
            {
                var clips = Animator.runtimeAnimatorController.animationClips;
                if (clips.Length > 0)
                {
                    var randomClip = FunctionHandler.GetRandomElementArray(clips);
                    randomClipName = randomClip.name;
                    Animator.Play(randomClipName);
                }
            }
            return randomClipName;
        }

        private void Update()
        {
            Play();
        }
    }
}
