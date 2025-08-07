using UnityEngine;

namespace Assets.Effects.Explosion
{
    public class GameEffect : MonoBehaviour
    {
        public Animator animator;           // ������ �� Animator
        public AudioSource audioSource;     // ������ �� AudioSource

        public void PlayEffect(string animation, string sound)
        {
            // ������ �������� � �����
            if (animator != null)
                animator.SetTrigger("Explode"); // ��������������, ��� � Animator ���� Trigger "Explode"

            if (audioSource != null)
                audioSource.Play();

            // ��������� �������� ��� �������� �������
            StartCoroutine(DestroyAfterEffect());
        }

        private System.Collections.IEnumerator DestroyAfterEffect()
        {
            float animDuration = GetAnimationLength("Explosion"); // �������� ��������
            float soundDuration = audioSource != null ? audioSource.clip.length : 0f;

            float delay = Mathf.Max(animDuration, soundDuration); // ���, ���� �� ���������� � ��, � ������
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
