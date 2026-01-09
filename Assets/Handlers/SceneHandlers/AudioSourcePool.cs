using System.Collections;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Handlers.SceneHandlers
{
    public class AudioSourcePool : ObjectPoolHandler
    {
        public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
        {
            var obj = Get();
            if (obj == null) return;

            obj.transform.position = position;
            var src = obj.GetComponent<AudioSource>();
            src.clip = clip;
            src.volume = volume;
            src.Play();

            StartCoroutine(ReturnAfter(src, clip.length));
        }

        private IEnumerator ReturnAfter(AudioSource src, float delay)
        {
            yield return new WaitForSeconds(delay);
            Return(src.gameObject);
        }
    }
}