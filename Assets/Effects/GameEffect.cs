using UnityEngine;

namespace Assets.Effects.Explosion
{
    public class GameEffect : MonoBehaviour
    {
        public Animator animator;           // Ссылка на Animator
        public AudioSource audioSource;     // Ссылка на AudioSource

        public void PlayEffect(string animation, string sound)
        {
            // Запуск анимации и звука
            if (animator != null)
                animator.SetTrigger("Explode"); // Предполагается, что в Animator есть Trigger "Explode"

            if (audioSource != null)
                audioSource.Play();

            // Запускаем корутину для удаления объекта
            StartCoroutine(DestroyAfterEffect());
        }

        private System.Collections.IEnumerator DestroyAfterEffect()
        {
            float animDuration = GetAnimationLength("Explosion"); // Название анимации
            float soundDuration = audioSource != null ? audioSource.clip.length : 0f;

            float delay = Mathf.Max(animDuration, soundDuration); // Ждём, пока не завершится и то, и другое
            yield return new WaitForSeconds(delay);

            Destroy(gameObject);
        }

        private float GetAnimationLength(string animationName)
        {
            if (animator == null) return 0f;

            RuntimeAnimatorController ac = animator.runtimeAnimatorController;
            foreach (var clip in ac.animationClips)
            {
                if (clip.name == animationName)
                    return clip.length;
            }
            return 0f;
        }
    }
}
